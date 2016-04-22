local ScriptType = {}

ScriptType.Types = {
	Server = 0,
	Local = 1,
	Module = 2,
}

local TypeNames = {
	[ScriptType.Types.Server] = "Script";
	[ScriptType.Types.Local] = "LocalScript";
	[ScriptType.Types.Module] = "ModuleScript";
}

function ScriptType.GetTypeFromInstance(instance)
	if instance:IsA("ModuleScript") then
		return ScriptType.Types.Module
	elseif instance:IsA("LocalScript") then
		return ScriptType.Types.Local
	elseif instance:IsA("Script") then
		return ScriptType.Types.Server
	else
		error(instance.ClassName.." is not supported")
	end
end

function ScriptType.GetClassFromType(scriptType)
	return TypeNames[scriptType]
end

return ScriptType