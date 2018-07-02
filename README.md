# PostLogToChatworkForWindows
ゲームサーバーとかのログをtailして、正規表現でフィルターして、チャットワークにぶん投げるWindowsアプリ。
Windowsでこういうシンプルなのが見つからなかったので作った。もしかしたらすでにいいのがあるのかもしれない...

## システム要件
* .NET Framework 4.5.2が動くこと
* Windows PowerShellが動くこと(バージョンとか互換とかは確認していません)

未だにPowerShell使わずにログをtailする方法がわからない...なお、FileSytemWatcherではいつものごとくうまくいかない模様

![image](https://user-images.githubusercontent.com/4087776/42144700-80edf852-7df7-11e8-86b2-98cf7cc9db2a.png)
