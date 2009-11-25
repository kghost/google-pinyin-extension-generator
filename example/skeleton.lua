-- do not forget to remove BOM, that's why I hate notepad.

-- dump : debug, return human readable string of object 'o'
function dump (o, level)
  return dump_r (o, level or 3)
end

function dump_r (o, level)
  local s = ""
  if type(o) == "table" and level ~= 0 then
    s = s .. "{"
    for key, value in pairs(o) do
      s = s .. key .. " = "
      s = s .. dump_r (value, level - 1) .. ", "
    end
    s = s .. "}"
  else
    if type(o) == "function" then
      s = s .. "#function"
    else
      s = s .. (o or "#nil")
    end
  end
  return s
end

debug = nil

function FSM (t)
  local fsm = {}
  for _,v in ipairs(t) do
    local old, event, new, action = v[1], v[2], v[3], v[4]
    if fsm[old] == nil then
      fsm[old] = {}
      -- blame google, setmetatable is NOT usable
      --setmetatable(fsm[old], {__index = function (o, k)
      --  return rawget(o, k) or o[-1]
      --end})
    end
    fsm[old][event] = {new = new, action = action}
  end
  return fsm
end

fsm_hr = FSM {
-- put generated FSM table here
}

function example (s)
  local str = ""
  local state = 0
  s:gsub(".", function (c)
    local fsm = (fsm_hr[state][c] or fsm_hr[state][-1])
    if debug == nil then
      if fsm then
        if fsm.action then
          str = str .. fsm.action
        end
      else
        str = str .. c
      end
      state = fsm and fsm.new or 0
    else
      if fsm == nil then
        str = str .. "Error: State[" .. state .. "] Char[" .. c .. "]\n"
        return str
      end
      str = str .. "S[" .. state .. "]" .. " C[" .. c .. "] " .. dump(fsm) .. "\n"
      str = str .. "R: " .. (fsm.action or "") .. "\n"
      state = fsm.new
    end
  end)
  return str
end

-- you known how to modify this line
ime.register_command("xx", "example", "just an example", "none")
