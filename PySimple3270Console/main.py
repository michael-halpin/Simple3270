import PySimple3270
from PySimple3270 import TnKey

#TODO Need to draw the screen on the emulator.
#TODO Write documentation.

#region Set object used to read and write from the mainframe.  These can alternatively be loaded from json text files.
fields = [{"Name": "Url", "X": 24, "Y": 5, "L": 31},
          {"Name": "Mainframe", "X": 21, "Y": 15, "L": 26},
          {"Name": "Text", "X": 16, "Y": 1, "L": 20}]

field = [{"Name": "Text", "X": 1, "Y": 1, "L": 2000}]

tso = [{"Name": "TSO", "X": 1, "Y": 24, "Value": "TSO"}]

wait1 = {"X": 10, "Y": 1, "Value": "IBM Z", "Timeout": 5000}
wait2 = {"X": 12, "Y": 1, "Value": "ENTER USERID -", "Timeout": 5000}
#endregion

# Get the Session Id of our mainframe connection.  This is used to make sure all requests get routed through this
# particular session that is active on the server side.
emu = PySimple3270.Emulator('A', '..\\Simple3270Console\\bin\Debug\\net5.0\\Simple3270Console.exe', 0)

# This code waits for the initial screen to load on the emulator.
if not emu.wait_for_text('A', wait1):
    raise Exception("Unable to connect.")

# This line reads the data at the specified fields off the emulator screen.
screen0 = emu.read_screen('A', fields)

# This code writes the data specified in the TSO object to the mainframe emulator.
if not emu.write_screen('A', tso):
    raise Exception("Error while writing data.")

# This code presses the specified "Enter" specified in the key variable.
if not emu.press_key('A', TnKey.Enter):
    raise Exception("Error while pressing key.")

# This code waits for the next screen to appear.
if not emu.wait_for_text('A', wait2):
    raise Exception("Timed out while waiting to load.")

# This line reads the data at the specified fields off the emulator screen.
screen1 = emu.read_screen('A', field)

# This line disconnects the emulator from the mainframe and disposes of the object.
emu.disconnect_session('A')