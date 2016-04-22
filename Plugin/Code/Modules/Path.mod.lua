-- Responsible for handling paths.
-------------------
----- Library -----
-------------------
local Path = {}

function Path.Normalize(pathString)
	-- Trim whitespace
	pathString = pathString:gsub("^%s+", "")
	pathString = pathString:gsub("%s+$", "")
	-- Remove anything with two consecutive separators
	pathString = pathString:gsub("%.%.+", ".")
	-- Remove any leading separators
	pathString = pathString:gsub("^%.+", "")
	-- Remove any trailing separators
	pathString = pathString:gsub("%.+$", "")
	
	return pathString
end

function Path.Join(path1, path2)
	return Path.Normalize(path1) + "." + Path.Normalize(path2)
end

function Path.MakeRelative(absolutePath, rootPath)
	absolutePath = Path.Normalize(absolutePath)
	rootPath = Path.Normalize(rootPath)
	
	local relativePath = absolutePath:gsub(rootPath..".", "")
	return relativePath
end

function Path.MakeAbsolute(relativePath, rootPath)
	relativePath = Path.Normalize(relativePath)
	rootPath = Path.Normalize(rootPath)
	
	local absolutePath = rootPath.."."..relativePath
	return absolutePath
end

function Path.Traverse(path, start, createMissing)
	if path:match("^game%.") then
		path = path:gsub("^game%.", "")
	end
	
	path = Path.Normalize(path)
	
	local chunks = {}
	for chunk in path:gmatch("[^%.]+") do
		table.insert(chunks, chunk)
	end
	
	local level = start
	
	for index, chunk in ipairs(chunks) do
		if index == #chunks then
			return level, chunk
		else
			local nextLevel = level:FindFirstChild(chunk)
			
			if not nextLevel then
				if createMissing then
					nextLevel = Instance.new("Folder")
					nextLevel.Name = chunk
					nextLevel.Parent = level
				else
					return false
				end
			end
			
			level = nextLevel
		end
	end
end

return Path