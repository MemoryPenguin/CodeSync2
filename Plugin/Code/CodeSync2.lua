-- squelch warnings
plugin = plugin

local NetworkBridge = require(script.Parent.Modules.NetworkBridge)
local UserInterfaceBridge = require(script.Parent.Modules.UserInterfaceBridge)
local SyncHandler = require(script.Parent.Modules.SyncHandler)

local function FormatTable(t, indentLevel)
	indentLevel = indentLevel or 0
	local indentStr = ("\t"):format():rep(indentLevel + 1)
	
	local result = "{\n"
	
	for key, value in pairs(t) do
		local strValue = tostring(value)
		if type(value) == "table" then
			strValue = FormatTable(value, indentLevel + 1)
		end
		
		result = result..("%s[%s] = %s,\n"):format(indentStr, key, strValue)
	end
	
	return result..("\t"):format():rep(indentLevel).."}"
end

local toolbar = plugin:CreateToolbar("CodeSync 2")
local button = toolbar:CreateButton("CodeSync 2", "Opens the CodeSync 2 user interface, allowing you to synchronize files across your computer and Studio.", "rbxassetid://347676411")
local open = false
button.Click:connect(function()
	open = not open
	button:SetActive(open)
	UserInterfaceBridge.SetUiOpen(open)
end)

local syncing = false

UserInterfaceBridge.OnTogglePressed:connect(function()
	syncing = false
	
	local port = UserInterfaceBridge.GetPort()
	
	if not port then
		UserInterfaceBridge.SetErrorMessage("Port should be a number between 1 and 65355")
		return
	end
	
	NetworkBridge.Port = port
	local success, info = pcall(NetworkBridge.GetInfo)
	if not success then
		UserInterfaceBridge.SetErrorMessage("Couldn't retrieve sync info; see output for more information")
		warn(info)
		return
	end
	
	local handler = SyncHandler.new(port, info.mappings)
	handler:Start()
	plugin:SetSetting("LastPort", port)
	
	local start = tick()
	syncing = start
	
	while syncing == start do
		wait(1)
		handler:Update()
	end
end)

UserInterfaceBridge.SetPort(plugin:GetSetting("LastPort") or 8453)