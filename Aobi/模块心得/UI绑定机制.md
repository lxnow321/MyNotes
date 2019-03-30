# UI绑定机制

## 1. 相关模块

1. UI相关模块：

    View.lua
    ViewBase.lua
    ListViewBase.lua
    ViewModelBase.lua

2. 绑定核心模块：

    DataBind.lua
    lockout.lua

3. 集成工具模块：
   
    underscore.lua

***

## 2. 核心思想

UI元素绑定property属性对象，订阅property属性(被订阅)变化事件，property属性的变化会通知所有订阅者（UI元素）

***

## 3. 方法实现解析


### 1. 创建属性property
   >MVVM模式ViewModel优先于View被创建。故property会先被创建。

        function ViewModelBase.createProperty( initvalue )
            return lo.observable(initvalue)
        end
   >lo.observable(initvalue)，创建一个observable(被观察者)table，并将initvalue作为初始值。

        lo.observable = function(initialValue)
            local observable = {
                _latestValue = initialValue
            }
            setmetatable(observable, lo.meta._observable)
            lo.subscribable(observable);
            return observable
        end
    
   >lo.observable方法中，创建了一个observable，增加了_observable被观察者特性，以及subscribable可被订阅的特性。

        _observable = {
            __call = function(self, ...)
                local arguments = {...}
                if (#arguments > 0) then
                    -- Write
                    -- Ignore writes if the value hasn't changed
                    -- self._latestValue ~= arguments[1] or type(arguments[1]) == 'table'
                    if ( not self.equalityComparer or not self.equalityComparer(self._latestValue, arguments[1]) ) then
                        self._latestValue = arguments[1];
                        self:valueHasMutated();
                    end
                    return self
                else
                    -- Read
                    -- lo.dependencyDetection.registerDependency(observable); -- The caller only needs to be notified of changes if they did a "read" operation
                    lo.dependencyDetection.registerDependency(self);
                    return self._latestValue;
                end
            end,
            ...
        }

   >_observable.__call中处理property()属性值返回，或者property(newvalue)修改属性值。当有传入值时，通过valueHasMutated()方法通知订阅者。注意：lo.dependencyDetection.registerDependency(self)方法内，将会处理自身与订阅者的绑定关系。此处流程暂时搁置，跳转到UI中的绑定过程，再回头梳理接下来的流程。

### 2. UI绑定

>XXView中一般使用self:BindValue将UI元素绑定到对应ViewModel中的property上（此处以value值绑定做说明）。

        function View:BindValue( component, property, valuename, ... )
            --property有三种：1、createProperty。2、createCollection。3、function。前两个类型为table。
            if not component or not property then
                local data = debug.getinfo(2)
                local whichFile = data.source
                local whichLine = data.currentline
                local str = ""
                if not component then
                    str = str.."component为空，"
                end
                if not property then
                    str = str.."property为空，"
                end
                printError("无法绑定值，"..str,whichFile,whichLine)
            end
            self.bindProperty[component] = self.bindProperty[component] or {}
            self.bindProperty[component][property] = self.bindProperty[component][property] or {}
            local computed = DataBind.BindingValue( component, property, valuename, ...)
            table.insert(self.bindProperty[component][property],computed)
        end

>self.bindProperty[component][property]列表中会存储通过DataBind.BindingValue( component, property, valuename,...)方法生成的computed对象(运算对象?)。

        function DataBind.BindingValue(target, path, valuename, ...)
            if valuename then
                if not ... then
                    return DataBind.CommonBind(target, path, valuename)
                else
                    local args = {...}
                    return DataBind.ComplexBind(target, path, valuename, args)
                end
            elseif ... ~= nil then
                local t = ...
                local btype = t.bindType
                if btype == bindType.Collection then
                    return DataBind.BindCollection(target, path, valuename,t)
                elseif btype == bindType.SetActive then
                    return DataBind.BindSetActive(target, path, valuename,t)
                elseif btype == bindType.ScrollRectCollection then
                    return DataBind.BindScrollRectCollection(target, path, valuename,t)
                elseif btype == bindType.SubView then
                    return DataBind.BindSubView(target,path,valuename,t)
                end
            end
        end

        ...
        function DataBind.CommonBind( target, path, valuename )
            return lo.computed(function() 
                        local value = path()
                        if DataBind.IsNil(value,valuename) then
                            return
                        end
                        if valuename == 'text' then
                            printError('computed function --', tostring(value))
                        end
                        target[valuename] = value
                    end)
        end

>此处lo.computed(callback)中的callback方法将会在被computed对象中的readFunction所引用。


        lo.computed = function (evaluatorFunctionOrOptions, options)
            local computed = {}

            local _latestValue
            local _hasBeenEvaluated = false
            local readFunction = evaluatorFunctionOrOptions

            function computed:evaluateImmediate()
            end
            function computed:set()
            end
            function computed:change()
                self:evaluateImmediate()
            end
            function computed:get()
            end
            ...
            lo.subscribable(computed);
            computed:evaluateImmediate();

            return computed;
        }

>调用lo.computed时，会默认执行一次computed:evaluateImmediate()

        function computed:evaluateImmediate()
            if (_hasBeenEvaluated and disposeWhen()) then
                self:dispose();
                return
            end

            local _stat, _err = xpcall(function()
                local disposalCandidates = u_.extend({},self._subscriptionsToDependencies)

                lo.dependencyDetection.begin(function(subscribable)
                    local inOld =  u_(disposalCandidates):detect(function(i)
                        return (i.target) == subscribable		    
                    end);
            
                    if not inOld then
                        --若property里边有此property，则会无限循环注册监听，导致这里无限注册，从而使lua内存一直增加，再dispose前不会被释放，也就是泄露了啊。
                        self._subscriptionsToDependencies[#(self._subscriptionsToDependencies)+1] = subscribable:subscribe(self); -- Brand new subscription - add it
                    end
                end);

                readFunction();
            end,__G__TRACKBACK__)
            lo.dependencyDetection['end']();
            _hasBeenEvaluated = true;
        end
    
>computed:evaluateImmediate中会执行lo.dependencyDetaction.begin(function()...end)
>这里begin中的回调方法就是将computed对象与observable对象绑定的过程。

>`注意：lo.dependencyDetaction.begin(callback)，传入的callback不是立即调用。而是在调用readFunction()时，调用通过DataBind.CommonBind等绑定方法传入的回调方法。`

        function DataBind.CommonBind( target, path, valuename )
            return lo.computed(function() 
                        local value = path()
                        if DataBind.IsNil(value,valuename) then
                            return
                        end
                        if valuename == 'text' then
                            printError('computed function --', tostring(value))
                        end
                        target[valuename] = value
                    end)
        end

>以DataBind.CommonBind绑定value值来说明：当调用readFunction()执行上述回调方法时，会调用一次path()，此处的path追溯源头即知path为ViewModel中property对象，也就是observable被观察者对象。 没有传值，则会调用lo.dependencyDetection.registerDependency(self)。此处的self表示observable自身。然后可以接着“1.创建属性property”的内容继续。

        lo.dependencyDetection = (function ()
            local _frames = {};

            return {
                begin = function (callback)
                    _frames[#_frames+1] = { callback = callback, distinctDependencies={} }
                end,

                ["end"] = function ()
                    _frames[#_frames] = nil
                end,

                registerDependency = function (subscribable)
                    
                    if not lo.isSubscribable(subscribable) then
                        error("Only subscribable things can act as dependencies");
                    end

                    if (#_frames > 0) then
                        -- printError('registerDependency function', tostring(#_frames))
                        local topFrame = _frames[#_frames];
                        if u_(u_.values(topFrame.distinctDependencies)):any(function(v) return v == subscribable end) then
                            return
                        end

                        topFrame.distinctDependencies[#(topFrame.distinctDependencies)+1] = subscribable;
                        topFrame.callback(subscribable);
                    end
                end
            }
        end)();

>lo.dependencyDetection.registerDependency(subscribable)，接受一个subscribable对象（observable在创建时，已经加入了subscribable特性，即可算作一个subscribable对象）。此处会判断一次#_frames>0。根据上述lo.dependencyDetection的代码机制。执行begin时，会插入一个_frame，执行end时，会删除一个_frame。所以一般情况下，只有在begin和end之间去执行registerDependency的方法，才有可能满足条件#_frames>0去执行内部的操作。
>回到上述computed:evaluateImmediate()方法解析可以看出，其中的执行满足begin->registerDependency->end顺序。_begin加入_frames信息，传入callback，执行readFunction()时，调用了一次lo.dependencyDetection.registerDependency()，从而执行topFrame.callback(subscribable)。此处topFrame.callback即为computed:evaluateImmediate()中调用lo.dependencyDetection.begin传入的回调。即执行如下：
        
    self._subscriptionsToDependencies[#(self._subscriptionsToDependencies)+1] = subscribable:subscribe(self);

>此处的self表示computed对象，subscribable是传入的observable对象。

    lo.subscription = function (t, c, event)
        local sst = {}

        sst.target = t
        sst.isDisposed = false
        sst.callback = c

        function sst:dispose()
            self.isDisposed = true;
            self.target._subscriptions[event][sst] = nil
        end

        return sst
    end

    ...

    lo.subscribable_fn = {
        subscribe = function (self, callbackOrTarget, event)
            event = event or defaultEvent;
            local boundCallback = type(callbackOrTarget) == 'table' and
                function(...)
                    callbackOrTarget[event](callbackOrTarget,...)
                end or callbackOrTarget;

            local subscription = lo.subscription(self, boundCallback, event);

            self._subscriptions[event] = self._subscriptions[event] or {}
            self._subscriptions[event][subscription] = true
            return subscription;
        end,
        ...
    }

>callbackOrTarget即上述的computed对象自身，由于该对象是个table，且默认event = defaultEvent = 'change'。那么boundCallback即为computed['change'](computed,...)，即调用computed:change方法。
>subscription = lo.subscription(self, boundCallback, event);生成一个被观察者和事件的订阅关系对象，由observable对象_subscriptions所持有。即observable._subscriptions可以看做是被观察者（被订阅者）中的订阅列表，当被观察者信息变化时，将会根据该订阅列表通知到各个订阅者对象。
    
    lo.subscribable_fn = {
        ...
        notifySubscribers = function (self, valueToNotify, event)
            event = event or defaultEvent;
            if (self._subscriptions[event]) then
                u_(u_.keys(self._subscriptions[event])):each( function(subscription)
                    if (subscription and subscription.isDisposed ~= true) then
                        --print("DataBind.CommonBind-1",valueToNotify)
                        subscription.callback(valueToNotify);
                    end
                end)
            end
        end,
        ...
    }

>observable信息变化时，通过调用self:notifySubscribers(self._latestValue)通知所有订阅者,subscription.callback，即为上述boundCallback回调方法，即一般默认情况下的computed:change(observable._latestValue)


`备注：只是大致梳理了一下流程，逻辑上可能还有些问题，文字组织上还需要进一步精炼。待调整。`