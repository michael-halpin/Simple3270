import requests


class Emulator:
    api = ''

    def __init__(self, api):
        self.api = api

    def establish_connection(self, config):
        api_url = self.api + "EstablishConnection"
        response = requests.post(api_url, json=config)
        session_id = response.text
        return session_id

    def read_screen(self, session_id, fields):
        api_url = self.api + "ReadScreen?sessionId=" + session_id
        response = requests.post(api_url, json=fields)
        packet = response.json()
        return packet

    def write_screen(self, session_id, fields):
        api_url = self.api + "WriteScreen?sessionId=" + session_id
        response = requests.post(api_url, json=fields)
        packet = response.json()
        return packet

    def wait_for_text(self, session_id, field):
        api_url = self.api + "WaitForText?sessionId=" + session_id
        response = requests.post(api_url, json=field)
        packet = response.json()
        return packet

    def press_key(self, session_id, key):
        api_url = self.api + "PressKey?sessionId=" + session_id
        response = requests.post(api_url, json=key)
        packet = response.json()
        return packet

    def disconnect_session(self, session_id):
        api_url = self.api + "DisconnectSession?sessionId=" + session_id
        response = requests.post(api_url, json='')
        if response.text == 'true':
            session_id = True
        else:
            session_id = False
        return session_id
