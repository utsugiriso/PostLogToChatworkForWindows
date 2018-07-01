# PostLogToChatworkForWindows
ゲームサーバーとかのログをtailして、正規表現でフィルターして、チャットワークにぶん投げるWindowsアプリ。
Windowsでこういうシンプルなのが見つからなかったので作った。もしかしたらすでにいいのがあるのかもしれない...

## システム要件
* .NET Framework 4.5.2が動くこと
* Windows PowerShellが動くこと(バージョンとか互換とかは確認していません)

未だにPowerShell使わずにログをtailする方法がわからない...なお、FileSytemWatcherではいつものごとくうまくいかない模様
