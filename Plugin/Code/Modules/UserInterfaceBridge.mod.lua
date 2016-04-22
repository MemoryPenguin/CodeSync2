-- Responsible for controlling the UI.

--------------------
----- Services -----
--------------------
local CoreGui = game:GetService("CoreGui")

---------------------
----- Constants -----
---------------------
local OPEN_SIZE = UDim2.new(0, 580, 0, 430)
local CLOSED_SIZE = UDim2.new(0, 580, 0, 129)

local OPEN_POSITION = UDim2.new(0.5, -290, 0.5, -215)
local CLOSED_POSITION = UDim2.new(0.5, -290, 0.5, -65)

local ENABLED_COLOR = Color3.new(32 / 255, 166 / 255, 148 / 255)
local DISABLED_COLOR = Color3.new(200 / 255, 200 / 255, 200 / 255)

---------------------
----- Variables -----
---------------------
local ui = script.CodeSync2
local container = ui.Container
local errorLabel = container.Input.Error
local toggleButton = container.Input.Toggle
local portInput = container.Input.Port.Box

local uiOpen = false
local infoOpen = false

-------------------
----- Library -----
-------------------
local UserInterfaceBridge = {}

function UserInterfaceBridge.SetUiOpen(open)
	ui.Parent = open and CoreGui or script
	uiOpen = open
end

function UserInterfaceBridge.HideUi()
	UserInterfaceBridge.SetUiOpen(false)
end

function UserInterfaceBridge.ShowUi()
	UserInterfaceBridge.SetUiOpen(true)
end

function UserInterfaceBridge.IsOpen()
	return uiOpen
end

function UserInterfaceBridge.SetInfoShown(shown)
	infoOpen = shown
	
	local newPosition, newSize
	
	if infoOpen then
		newPosition = OPEN_POSITION
		newSize = OPEN_SIZE
	else
		newPosition = CLOSED_POSITION
		newSize = CLOSED_SIZE
	end
	
	if uiOpen then
		container:TweenPosition(newPosition, Enum.EasingDirection.InOut, Enum.EasingStyle.Sine, 0.15, true)
		container:TweenSize(newSize, Enum.EasingDirection.InOut, Enum.EasingStyle.Sine, 0.15, true)
	else
		container.Position = newPosition
		container.Size = newSize
	end
end

function UserInterfaceBridge.ShowInfo()
	UserInterfaceBridge.SetInfoShown(true)
end

function UserInterfaceBridge.HideInfo()
	UserInterfaceBridge.SetInfoShown(false)
end

function UserInterfaceBridge.SetErrorMessage(message)
	errorLabel.Text = message
end

function UserInterfaceBridge.FreezeUi()
	portInput.Enabled = false
	toggleButton.Enabled = false
	toggleButton.BackgroundColor3 = DISABLED_COLOR
end

function UserInterfaceBridge.ThawUi()
	portInput.Enabled = true
	toggleButton.Enabled = true
	toggleButton.BackgroundColor3 = ENABLED_COLOR
end

function UserInterfaceBridge.GetPort()
	return tonumber(portInput.Text)
end

function UserInterfaceBridge.SetPort(port)
	portInput.Text = port
end

function UserInterfaceBridge.SetToggleText(text)
	toggleButton.Text = text
end

UserInterfaceBridge.OnTogglePressed = toggleButton.MouseButton1Down

return UserInterfaceBridge