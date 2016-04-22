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
	print("sync starting!")
	NetworkBridge.Port = self.Port
	print("network bridge acting on port "..self.Port)
	
	for index, mapping in ipairs(self.MappingInfo) do
		print("acquiring object for mapping #"..index.." ("..mapping["absoluteDisk"].." <-> "..mapping["roblox"]..")")
		local parent, name = Path.Traverse(mapping["roblox"], game, true)
		
		local object = parent:FindFirstChild(name)
		if not object then
			object = Instance.new("Folder", parent)
			object.Name = name
		end
		
		print("acquired object!")
		
		local realMapping = {
			Object = object;
			RobloxPath = mapping["roblox"];
			FsPath = mapping["absoluteDisk"];
		}
		
		table.insert(self.Mappings, realMapping)
		print("mapping initialized")
	end
	
	-- TODO: only clear if sync authority is FS
	print("clearing mapping objects")
	for _, mapping in ipairs(self.Mappings) do
		mapping.Object:ClearAllChildren()
	end
	
	print("reading scripts")
	for _, scriptInfo in ipairs(NetworkBridge.GetScripts()) do
		print("initial sync: "..scriptInfo.RobloxPath)
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
	
	print("initial sync is done")
end

function SyncHandler:Update()
	local changes = NetworkBridge.GetChanges()
	print("replicating "..#changes.." change(s)")
	
	for _, change in ipairs(changes) do
		print("change to "..change.ChangedScript.RobloxPath.." of type "..change.Type)
		
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
			end
			
			if object.Source ~= change.NewContent then
				object.Source = change.NewContent
				print("wrote to "..change.ChangedScript.RobloxPath)
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
			
			print("removed "..change.ChangedScript.RobloxPath)
		-- ???
		else
			warn("unknown change type: "..change.Type)
		end
	end
	
	print("done updating")
end

return SyncHandler