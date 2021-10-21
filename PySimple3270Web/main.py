from PySimple3270 import Emulator

# This line creates an object used to interact with the mainframe emulator on the server.
Emu = Emulator("http://localhost:5000/")

#region Set object used to read and write from the mainframe.  These can alternatively be loaded from json text files.
config = {"server": "192.86.32.153", "port": 623, "lu": "", "useSsl": False, "fastScreenMode": True,
          "terminalType": "IBM-3278-2-E", "debug": False, "actionTimeout": 1000, "colorDepth": 2}
fields = [{"Name": "Url", "X": 24, "Y": 5, "L": 31},
          {"Name": "Mainframe", "X": 21, "Y": 15, "L": 26},
          {"Name": "Text", "X": 16, "Y": 1, "L": 20}]
field = [{"Name": "Text", "X": 1, "Y": 1, "L": 2000}]
tso = [{"Name": "TSO", "X": 1, "Y": 24, "Value": "TSO"}]
key = "Enter"
wait1 = {"X": 10, "Y": 1, "Value": "IBM Z", "Timeout": 5000}
wait2 = {"X": 12, "Y": 1, "Value": "ENTER USERID -", "Timeout": 5000}
#endregion

# Get the Session Id of our mainframe connection.  This is used to make sure all requests get routed through this
# particular session that is active on the server side.
session_id = Emu.establish_connection(config)

# This code waits for the initial screen to load on the emulator.
if not Emu.wait_for_text(session_id, wait1):
    raise Exception("Unable to connect.")

# This line reads the data at the specified fields off the emulator screen.
screen1 = Emu.read_screen(session_id, fields)

# This code writes the data specified in the TSO object to the mainframe emulator.
if not Emu.write_screen(session_id, tso):
    raise Exception("Error while writing data.")

# This code presses the specified "Enter" specified in the key variable.
if not Emu.press_key(session_id, key):
    raise Exception("Error while pressing key.")

# This code waits for the next screen to appear.
if not Emu.wait_for_text(session_id, wait2):
    raise Exception("Timed out while waiting to load.")

# This line reads the data at the specified fields off the emulator screen.
screen2 = Emu.read_screen(session_id, field)


# This line disconnects the emulator from the mainframe and disposes of the object.
Emu.disconnect_session(session_id)
print('')