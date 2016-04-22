-- A Lua-controlled event.

-----------------
----- Class -----
-----------------
local Signal = {}
Signal.__index = Signal

function Signal.new()
	local self = setmetatable({
		_args = {};
		_object = Instance.new("BindableEvent");
	}, Signal)
	
	return self
end

function Signal:Connect(listener)
	return self._object.Event:connect(function()
		listener(unpack(self._args))
	end)
end
Signal.connect = Signal.Connect

function Signal:Fire(...)
	self._args = {...}
	self._object:Fire()
end

function Signal:Wait()
	self._object.Event:wait()
	return unpack(self._args)
end

return Signal