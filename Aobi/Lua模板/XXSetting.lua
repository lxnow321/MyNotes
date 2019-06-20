module('AQ.Share', package.seeall)

ShareSetting = SingletonClass('ShareSetting')

local instance = ShareSetting

local PMShareInfoConfig = require('Commons/Config/Share/PMShareInfoConfig')

function ShareSetting.Init()
	instance.PMShareInfoConfig = AQ.ConfigUtil.Convert(PMShareInfoConfig, {'raceId'})
end

function ShareSetting.GetPMShareInfoConfig(raceId)
	local config = instance.PMShareInfoConfig
	return config and config[raceId]
end
