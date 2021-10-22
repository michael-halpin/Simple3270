import os
import random
import string
import _Steganography
from _Win32_Comm import Pipe


class TnKey:
    F1 = "F1"
    F2 = "F2"
    F3 = "F3"
    F4 = "F4"
    F5 = "F5"
    F6 = "F6"
    F7 = "F7"
    F8 = "F8"
    F9 = "F9"
    F10 = "F10"
    F11 = "F11"
    F12 = "F12"
    F13 = "F13"
    F14 = "F14"
    F15 = "F15"
    F16 = "F16"
    F17 = "F17"
    F18 = "F18"
    F19 = "F19"
    F20 = "F20"
    F21 = "F21"
    F22 = "F22"
    F23 = "F23"
    F24 = "F24"
    Tab = "Tab"
    BackTab = "BackTab"
    Enter = "Enter"
    Backspace = "Backspace"
    Clear = "Clear"
    Delete = "Delete"
    DeleteField = "DeleteField"
    DeleteWord = "DeleteWord"
    Left = "Left"
    Left2 = "Left2"
    Up = "Up"
    Right = "Right"
    Right2 = "Right2"
    Down = "Down"
    Attn = "Attn"
    CircumNot = "CircumNot"
    CursorSelect = "CursorSelect"
    Dup = "Dup"
    Erase = "Erase"
    EraseEOF = "EraseEOF"
    EraseInput = "EraseInput"
    FieldEnd = "FieldEnd"
    FieldMark = "FieldMark"
    FieldExit = "FieldExit"
    Home = "Home"
    Insert = "Insert"
    Interrupt = "Interrupt"
    Key = "Key"
    Newline = "Newline"
    NextWord = "NextWord"
    PAnn = "PAnn"
    PreviousWord = "PreviousWord"
    Reset = "Reset"
    SysReq = "SysReq"
    Toggle = "Toggle"
    ToggleInsert = "ToggleInsert"
    ToggleReverse = "ToggleReverse"
    PA1 = "PA1"
    PA2 = "PA2"
    PA3 = "PA3"
    PA4 = "PA4"
    PA5 = "PA5"
    PA6 = "PA6"
    PA7 = "PA7"
    PA8 = "PA8"
    PA9 = "PA9"
    PA10 = "PA10"
    PA11 = "PA11"
    PA12 = "PA12"


class Emulator:
    exe = ''
    key = ''
    iv = ''
    session = ''
    keys = ["F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15", "F16",
                "F17", "F18", "F19", "F20", "F21", "F22", "F23", "F24", "Tab", "BackTab", "Enter", "Backspace", "Clear",
                "Delete", "DeleteField", "DeleteWord", "Left", "Left2", "Up", "Right", "Right2", "Down", "Attn",
                "CircumNot", "CursorSelect", "Dup", "Erase", "EraseEOF", "EraseInput", "FieldEnd", "FieldMark",
                "FieldExit", "Home", "Insert", "Interrupt", "Key", "Newline", "NextWord", "PAnn", "PreviousWord",
                "Reset", "SysReq", "Toggle", "ToggleInsert", "ToggleReverse", "PA1", "PA2", "PA3", "PA4", "PA5", "PA6",
                "PA7", "PA8", "PA9", "PA10", "PA11", "PA12"]


    def __init__(self, session, exe, config):
        self.exe = exe
        self.session = session
        self.key = _rand(32, 2)
        self.iv = _rand(16, 2)
        self.key = "99999999999999999999999999999999"
        self.iv = "0000000000000000"
        #os.system("start " + exe + " " + session + " " + self.key + " " + self.iv + " " + config)
        # Launch app here.

    def read_screen(self, session_id, fields):
        package = _Steganography.package(str(fields), 'read_screen', self.key, self.iv)
        packet = Pipe.query(package, session_id, self.key, self.iv)
        return packet

    def write_screen(self, session_id, fields):
        package = _Steganography.package(str(fields), 'write_screen', self.key, self.iv)
        packet = Pipe.query(package, session_id, self.key, self.iv)
        return packet

    def wait_for_text(self, session_id, field):
        package = _Steganography.package(str(field), 'wait_for_text', self.key, self.iv)
        packet = Pipe.query(package, session_id, self.key, self.iv)
        return packet

    def press_key(self, session_id, key):
        matched = False
        for button in self.keys:
            if str(key).lower() == button.lower():
                key = button
                matched = True
                break

        if not matched:
            raise Exception("The key '" + key + "' is not on the list of approved keys.")

        package = _Steganography.package('{"key":"' + key + '"}', 'press_key', self.key, self.iv)
        packet = Pipe.query(package, session_id, self.key, self.iv)
        return packet

    def disconnect_session(self, session_id):
        package = _Steganography.package('{"content":"disconnect_session"}', 'disconnect_session', self.key, self.iv)
        Pipe.set(session_id, package)
        return True

def _rand(count, level):
    """
    Generates a random string.
    :param count: The number of characters to generate.
    :param level: The level of complexity in generated string.
    :return: string
    """
    text = ""
    symbols = ['!', '@', '#', '$', '%', '(', ')', '-', '_', '=', '+', '[', ']', '~', ',']  # Can add more
    for _ in range(count):
        r = random.randint(0, level)
        if r == 0:
            text += random.choice(string.ascii_lowercase)
        elif r == 1:
            text += random.choice(string.ascii_uppercase)
        elif r == 2:
            text += random.choice(string.digits)
        elif r == 3:
            text += random.choice(symbols)
    return text
