-- Responsible for interacting with the file system web server.

--------------------
----- Services -----
--------------------
local HttpService = game:GetService("HttpService")

---------------------
----- Constants -----
---------------------
-- How many requests can be sent in a minute. Exceeding 500 will cause HttpService stalls.
-- A limit less than 500 ensures that other HttpService-using plugins won't be drowned out by this.
local REQUEST_LIMIT = 250

-- How long to wait after a HttpService call limit overrun.
local REQUEST_LIMIT_COOLDOWN = 15

-- How many times to (re)try a request before failing. Should stop one-off HttpService errors.
local REQUEST_TRIES = 2

-- How long a request is considered 'active' and counting towards the limit
local REQUEST_COOLDOWN = 60

-- How long, in seconds, between request cooldown steps
local STEP_DELAY = 1

-- Local URL
local BASE_URL = "http://localhost:%d/%s?%s"

---------------------
----- Variables -----
---------------------
-- How many queries were executed in the past 60 seconds
local requestsUsed = 0

-- How many HTTP errors have we encountered
local errorCount = 0

-- Records the times (via tick) of requests so the usage counter can be updated
local requestTimes = {}

-- Whether the step thread is currently running
local stepping = false

---------------------
----- Functions -----
---------------------
local function StepQueue()
	local current = tick()
	
	for i = #requestTimes, 1, -1 do
		local requestTime = requestTimes[i]
		
		if current - requestTime > REQUEST_COOLDOWN then
			requestsUsed = requestsUsed - 1
			table.remove(requestTimes, i)
		end
	end
end

local function TryStartStepper()
	if stepping then
		return
	end
	
	spawn(function()
		stepping = true
		
		while stepping do
			StepQueue()
			stepping = #requestTimes > 0
			wait(STEP_DELAY)
		end
	end)
end

local function PushRequestTime(requestTime)
	requestsUsed = requestsUsed + 1
	table.insert(requestTimes, requestTime)
	TryStartStepper()
end

local function MakeRequest(queryUrl)
	-- todo: better method?
	if requestsUsed > REQUEST_LIMIT then
		while requestsUsed > REQUEST_LIMIT do
			wait(0)
		end
	end
	
	local success, message
	local failCount = 0
	
	while failCount < REQUEST_TRIES do
		-- rather than making a closure or a new function, just call it with HttpService
		success, message = pcall(HttpService.GetAsync, HttpService, queryUrl, true)
		PushRequestTime(tick())
		
		if not success then
			-- Didn't just run into a HttpService stall
			if message ~= "Number of requests exceeded limit" then
				errorCount = errorCount + 1
				failCount = failCount + 1
			else
				wait(REQUEST_LIMIT_COOLDOWN)
			end
		else
			break
		end
	end
	
	return success, message
end

local function FormatUrl(port, target, arguments)
	local argumentString = ""
	
	if arguments then
		-- Temporary dumping table
		local argumentStrings = {}
		
		-- Format arguments in normal form
		for key, value in pairs(arguments) do
			table.insert(argumentStrings, key.."="..HttpService:UrlEncode(tostring(value)))
		end
		
		-- Generate argument string
		argumentString = table.concat(argumentStrings, "&")
	end
	
	return BASE_URL:format(port, target, argumentString)
end

-------------------
----- Library -----
-------------------
local NetworkBridge = {}

NetworkBridge.Port = 8453

--- Summary ---
-- Gets the current synchronization information, consisting of:
-- * Mappings
-- * Port
-- * Sync mode and authority
--- Returns ---
-- [dictionary<string, variant>]: A dictionary containing information on the sync.
function NetworkBridge.GetInfo()
	local success, message = MakeRequest(FormatUrl(NetworkBridge.Port, "info"))
	
	if success then
		local response = HttpService:JSONDecode(message)
		local data = response.Data
		return data
	else
		error("Error retrieving info: "..message)
	end
end

--- Summary ---
-- Gets the current scripts of the file system server.
--- Returns ---
-- An array of all the scripts.
function NetworkBridge.GetScripts()
	local success, message = MakeRequest(FormatUrl(NetworkBridge.Port, "scripts"))
	
	if success then
		local response = HttpService:JSONDecode(message)
		local data = response.Data
		return data
	else
		error("Error retrieving scripts: "..message)
	end
end

function NetworkBridge.GetChanges()
	local success, message = MakeRequest(FormatUrl(NetworkBridge.Port, "changes"))
	
	if success then
		local response = HttpService:JSONDecode(message)
		local data = response.Data
		return data
	else
		error("Error retrieving scripts: "..message)
	end
end

function NetworkBridge.ReadFileAt(fullName)
	local args = { location = fullName; }
	local success, message = MakeRequest(FormatUrl(NetworkBridge.Port, "read", args))
	
	if success then
		local response = HttpService:JSONDecode(message)
		local data = response.Data
		return data
	else
		error("Error retrieving script contents: "..message)
	end
end

function NetworkBridge.GetInfo()
	local success, message = MakeRequest(FormatUrl(NetworkBridge.Port, "info"))
	
	if success then
		local response = HttpService:JSONDecode(message)
		local data = response.Data
		return data
	else
		error("Error getting info: "..message)
	end
end

function NetworkBridge.GetRequestQueue()
	return requestsUsed
end

function NetworkBridge.GetMaxRequests()
	return REQUEST_LIMIT
end

function NetworkBridge.GetErrors()
	return errorCount
end

return NetworkBridge