import sys
from re import search
import joblib
import os


jsMeta = [r'cmd', r'cmd.exe', r'exe', r'eval', r'window',
          r'[sS][yY][sS][tT][eE][mM]', r'[rR]un', r'power[\w\W]*shell', r'shell', r'.com',
          r'split', r'Object', r'WScript', r'XML', r'hHTTP',
          r'[gG][eE][tT]', r'open', r'send', r'response', r'[uU]ser',
          r'path', r'C:\\', r'[wW][iI][nN]', r'[pP]rocess', r'[bB]ypass',
          r'[sS]tart', r'[Dd]o', r'[sS]et', r'case', r'[eE][oO][fF]',
          r'function', r'var', r'script', r'getAttribute', r'currentScript',
          r'class', r'contentWindow', r'document', r'addEventListener', r'attachEvent',
          r'close', r'[rR]ead', r'key', r'decodeURIComponent', r'[jJ][sS][oO][nN]',
          r'cookie', r'options', r'argument', r'stringifyCookieValue', r'[eE]vent',
          r'currentTarget', r'Policy', r'[-+]?(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][-+]?\d+)?', r'length', r'ActiveXObject',
          r'join', r'try', r'catch', r'http', r'.org']



if __name__ == "__main__":
    if len(sys.argv) != 2:
        sys.exit(0)
    if len(sys.argv) == 2:
        file = sys.argv[1]
        res = 0
        hand_x = []

        handle = open(file, "rb")
        data = str(handle.read())
        for jsm in jsMeta:
            matches = None
            try:
                matches = search(jsm, data)
            except:
                matches = None
            if matches != None:
                hand_x.append(1)
            else:
                hand_x.append(0)

        forest = joblib.load('C:\\Users\\Julia\\Downloads\\5\\learn.pkl')
        res = forest.predict([hand_x])
        print(int(res[0]))
        sys.exit(int(res[0]))



