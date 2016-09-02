local NetworkBridge = require(script.Parent.NetworkBridge)
local Path = require(script.Parent.Path)
local ScriptType = require(script.Parent.ScriptType)

local function RemoveEmptyFolders(level)
	while level:GetChildren() <= 0 do
		local nextLevel = level.Parent
		level:Destroy()
		level = nextLevel
	end
end

local SyncHandler = {}
SyncHandler.__index = SyncHandler

function SyncHandler.new(port, mappings)
	local self = setmetatable({}, SyncHandler)
	self.MappingInfo = mappings
	self.Mappings = {}
	self.Port = port
	self.Scripts = {}
	self.IsSyncing = false
	
	return self
end

function SyncHandler:Start()
	NetworkBridge.Port = self.Port
	print("Network bridge acting on port "..self.Port)
	
	for index, mapping in ipairs(self.MappingInfo) do
		local parent, name = Path.Traverse(mapping["roblox"], game, true)
		
		local object = parent:FindFirstChild(name)
		if not object then
			object = Instance.new("Folder", parent)
			object.Name = name
		end
		
		local realMapping = {
			Object = object;
			RobloxPath = mapping["roblox"];
			FsPath = mapping["absoluteDisk"];
		}
		
		table.insert(self.Mappings, realMapping)
		print("Initialized mapping #"..index.." ("..mapping["absoluteDisk"].." <-> "..mapping["roblox"]..")")
	end
	
	-- TODO: only clear if sync authority is FS
	print("Clearing contents of mapping objects.")
	for _, mapping in ipairs(self.Mappings) do
		mapping.Object:ClearAllChildren()
	end
	
	print("Reading scripts")
	for _, scriptInfo in ipairs(NetworkBridge.GetScripts()) do
		print("Performing initial sync of "..scriptInfo.FilePath.." to "..scriptInfo.RobloxPath)
		local parent, name = Path.Traverse(scriptInfo.RobloxPath, game, true)
		local content = NetworkBridge.ReadFileAt(scriptInfo.RobloxPath)
		local instanceClass = ScriptType.GetClassFromType(scriptInfo.Type)
		local object = parent:FindFirstChild(name)
		
		if object and not object:IsA(instanceClass) then
			object:Destroy()
			object = nil
		end
		
		if not object then
			object = Instance.new(instanceClass, parent)
			object.Name = name
		end
		
		object.Source = content
	end
end

function SyncHandler:Update()
	local changes = NetworkBridge.GetChanges()
	for _, change in ipairs(changes) do
		-- Write
		if change.Type == 0 then
			local parent, name = Path.Traverse(change.ChangedScript.RobloxPath, game, true)
			local content = NetworkBridge.ReadFileAt(change.ChangedScript.RobloxPath)
			local instanceClass = ScriptType.GetClassFromType(change.ChangedScript.Type)
			local object = parent:FindFirstChild(name)
			
			if object and not object:IsA(instanceClass) then
				object:Destroy()
				object = nil
			end
			
			if not object then
				object = Instance.new(instanceClass, parent)
				object.Name = name
				print("Made object for "..change.ChangedScript.RobloxPath)
			end
			
			if object.Source ~= change.NewContent then
				object.Source = change.NewContent
				print("Wrote to "..change.ChangedScript.RobloxPath)
			end
		-- Delete
		elseif change.Type == 1 then
			local parent, name = Path.Traverse(change.ChangedScript.RobloxPath, game)
			
			-- possible that the parent got destroyed here too
			if parent and parent:FindFirstChild(name) then
				parent[name]:Destroy()
				
				-- crawl upwards
				RemoveEmptyFolders(parent)
			end
			
			print("Removed "..change.ChangedScript.RobloxPath)
		-- ???
		else
			warn("Unknown change type: "..change.Type)
		end
	end
end

return SyncHandler