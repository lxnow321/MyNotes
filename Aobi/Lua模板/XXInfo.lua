module('AQ.House', package.seeall)

BoxMsgItem = class('BoxMsgItem')

function BoxMsgItem:ctor(letterType, content)
    self.letterType = letterType
    self.content = content
end

function BoxMsgItem.Clone(data)
    return BoxMsgItem.New(data.letterType, data.content)
end

