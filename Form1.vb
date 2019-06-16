Imports System
Imports System.IO
Imports System.Net
Imports System.IO.Compression
Imports System.Security.Principal
Imports MySql.Data.MySqlClient
Imports System.Security.Cryptography
Imports System.Text
Imports System.ComponentModel
Imports MaterialSkin
Imports System.Text.RegularExpressions

Public Class Form1
    Dim identity = WindowsIdentity.GetCurrent()
    Dim principal = New WindowsPrincipal(identity)
    Dim fivempath As String = "C:\HomeWin\Servers\FiveM\"
    Dim isElevated As Boolean = principal.IsInRole(WindowsBuiltInRole.Administrator)
    Dim tool As String = dirPath + "\Updater.exe"
    Dim wClient As New System.Net.WebClient
    Dim dirPath As String = My.Application.Info.DirectoryPath
    Private CurrentProcessID As Integer = -1
    Private Sub PictureBox8_Click(sender As Object, e As EventArgs) Handles PictureBox8.Click
        DeleteServer.ShowDialog()
    End Sub

    Private Sub Label17_Click(sender As Object, e As EventArgs) Handles Label17.Click
        DeleteServer.ShowDialog()
    End Sub


    Public Function GetDirSize(RootFolder As String) As String
        'Dim TotalSize As Long
        'Dim DoubledValue As Double
        'Dim FolderInfo = New IO.DirectoryInfo(RootFolder)
        'For Each File In FolderInfo.GetFiles : TotalSize += File.Length
        'Next
        'For Each SubFolderInfo In FolderInfo.GetDirectories : GetDirSize(SubFolderInfo.FullName)
        'Next

        Dim fso = CreateObject("Scripting.FileSystemObject")
        Dim profile = fso.GetFolder(RootFolder)
        Dim TotalSize = profile.Size
        Dim DoubledValue As Double

        Try
            Select Case TotalSize
                Case Is >= 1099511627776
                    DoubledValue = CDbl(TotalSize / 1099511627776) 'TB
                    Return FormatNumber(DoubledValue, 2) & " TB"
                Case 1073741824 To 1099511627775
                    DoubledValue = CDbl(TotalSize / 1073741824) 'GB
                    Return FormatNumber(DoubledValue, 2) & " GB"
                Case 1048576 To 1073741823
                    DoubledValue = CDbl(TotalSize / 1048576) 'MB
                    Return FormatNumber(DoubledValue, 2) & " MB"
                Case 1024 To 1048575
                    DoubledValue = CDbl(TotalSize / 1024) 'KB
                    Return FormatNumber(DoubledValue, 2) & " KB"
                Case 0 To 1023
                    DoubledValue = TotalSize ' bytes
                    Return FormatNumber(DoubledValue, 2) & " B"
                Case Else
                    Return ""
            End Select
        Catch
            Return ""
        End Try
    End Function

    Private Sub PictureBox7_Click(sender As Object, e As EventArgs) Handles PictureBox7.Click
        Dim fivempath As String = "C:\HomeWin\Servers\FiveM\resources"
        If System.IO.Directory.Exists(fivempath) Then
            Process.Start("C:\HomeWin\Servers\FiveM")
        Else
            Dim dirfivem As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM")
            If dirfivem.Exists Then
                dirfivem.Delete(True)
            End If
            Me.Close()
            MsgBox("FiveM Server not found!")
        End If
    End Sub


    Private Sub Label14_Click(sender As Object, e As EventArgs) Handles Label14.Click
        Dim fivempath As String = "C:\HomeWin\Servers\FiveM\resources"
        If System.IO.Directory.Exists(fivempath) Then
            Process.Start("C:\HomeWin\Servers\FiveM")
        Else
            Dim dirfivem As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM")
            If dirfivem.Exists Then
                dirfivem.Delete(True)
            End If
            Me.Close()
            MsgBox("FiveM Server not found!")
        End If
    End Sub

    Public Sub VerificarInternet()
        Try
            Using Client = New System.Net.WebClient()
                Using stream = Client.OpenRead("http://www.google.pt")
                    checkUpdates()
                    WebBrowser1.Stop()
                    WebBrowser2.Stop()
                End Using
            End Using
        Catch
            MsgBox("We didnt find any connection to the internet. App will close", MsgBoxStyle.Information)
            Me.Close()
        End Try
    End Sub

    Public Sub checkUpdates()
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("https://drive.google.com/uc?authuser=0&id=1k3pJEPkpS20IASi3ReE7US2HsiJKRC_B&export=download")
        Dim response As System.Net.HttpWebResponse = request.GetResponse()

        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

        Dim newestversion As String = sr.ReadToEnd()
        Dim currentversion As String = Application.ProductVersion

        If newestversion.Contains(currentversion) Then


            Dim dirfivem As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM\resources")
            If dirfivem.Exists Then
                getpublicip()
                GetDiskUsage.Start()
                GetDiskUsage.Interval = 2500
                CheckForLogins()
                GetServerStatus.Start()
                CheckServerExist.Start()
                Timer8.Start()
            Else
                ServerSlot.Text = "32"
                ServerList.Text = "Yes"
                Panel8.Hide()
                Panel9.Show()
                TabControl1.SelectedIndex = 10
                Timer3.Start()
            End If

        Else

            wClient.DownloadFileAsync(New Uri("https://ia801403.us.archive.org/24/items/Updater_20190306/Updater.exe"), dirPath + "\Updater.exe")
            Timer2.Start()

        End If
    End Sub

    Private Sub GetDiskUsage_Tick(sender As Object, e As EventArgs) Handles GetDiskUsage.Tick
        Label8.Text = GetDirSize("C:\HomeWin\Servers\FiveM")
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If isElevated Then
            VerificarInternet()
        Else
            MsgBox("You must open Server Creator with administrator.", MsgBoxStyle.Critical, "Error!")
            Application.Exit()
        End If
    End Sub

    Public Sub getpublicip()
        Dim client As New WebClient
        '// Add a user agent header in case the requested URI contains a query.
        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR1.0.3705;)")
        Dim baseurl As String = "http://checkip.dyndns.org/"
        ' with proxy server only:
        Dim proxy As IWebProxy = WebRequest.GetSystemWebProxy()
        proxy.Credentials = CredentialCache.DefaultNetworkCredentials
        client.Proxy = proxy
        Dim data As Stream
        Try
            data = client.OpenRead(baseurl)
        Catch ex As Exception
            Exit Sub
        End Try
        Dim reader As StreamReader = New StreamReader(data)
        Dim s As String = reader.ReadToEnd()
        data.Close()
        reader.Close()
        s = s.Replace("<html><head><title>Current IP Check</title></head><body>", "").Replace("</body></html>", "").ToString()
        'MessageBox.Show(s)
        Label3.Text = s
#Disable Warning BC40000 ' Type or member is obsolete
        Dim strIPAddress As String = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName).AddressList(0).ToString
#Enable Warning BC40000 ' Type or member is obsolete
        Label4.Text = strIPAddress
    End Sub

    Private Sub GetServerStatus_Tick(sender As Object, e As EventArgs) Handles GetServerStatus.Tick
        Dim p() As Process
        p = Process.GetProcessesByName("FXServer")
        If p.Count > 0 Then
            Label6.Text = "Online ✔"
            Label6.ForeColor = Color.SeaGreen
        Else
            Label6.Text = "Offline ❌"
            Label6.ForeColor = Color.Crimson
        End If
    End Sub

    Private Sub PictureBox5_Click(sender As Object, e As EventArgs) Handles PictureBox5.Click
        TabControl1.SelectedIndex = 2
    End Sub


    Private Sub Label12_Click(sender As Object, e As EventArgs) Handles Label12.Click
        TabControl1.SelectedIndex = 2
    End Sub

    Private Sub BunifuThinButton29_Click(sender As Object, e As EventArgs)
        Dim p() As Process
        p = Process.GetProcessesByName("FXServer")
        If p.Count > 0 Then

        Else
            For Each proc As Process In Process.GetProcesses
                If proc.ProcessName = "FXServer" Then

                Else
                    StartServerProcess.Start()

                End If
            Next
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles StartServerProcess.Tick
        StartServerProcess.Stop()
        CheckPlayerList.Start()
        StartProcess("C:\HomeWin\Servers\FiveM\startserver.cmd", "stackoverflow.com")
    End Sub

    Private Sub BunifuThinButton210_Click(sender As Object, e As EventArgs)
        Dim p() As Process
        p = Process.GetProcessesByName("FXServer")
        For Each proc As Process In Process.GetProcesses
            If proc.ProcessName = "FXServer" Then
                StartServerProcess.Start()
                proc.Kill()
                RichTextBox1.Text = ""
            End If
        Next
    End Sub

    Dim MyProcess As Process

    Private Sub SendCommand(cmd As String)
        MyProcess.StandardInput.WriteLine(cmd)
    End Sub

    Private Sub SendCommand(myprocess As Process, argumentes As String)
        Dim pi As ProcessStartInfo = New ProcessStartInfo()
        pi.Arguments = argumentes
        myprocess.StartInfo = pi
        myprocess.Start()
    End Sub


    Private Sub StartProcess(FileName As String, Arguments As String)
        MyProcess = New Process()

        Dim MyStartInfo As New ProcessStartInfo() With {
            .FileName = FileName,
            .Arguments = Arguments,
            .WorkingDirectory = Path.GetDirectoryName(FileName),
            .RedirectStandardError = True,
            .RedirectStandardOutput = True,
            .RedirectStandardInput = True,
            .UseShellExecute = False,
            .CreateNoWindow = True
        }

        'Dim MyProcess As Process = New Process() With {
        '    .StartInfo = MyStartInfo,
        '    .EnableRaisingEvents = True,
        '    .SynchronizingObject = Me
        '}

        'MyProcess = New Process() With {
        '     .StartInfo = MyStartInfo,
        '     .EnableRaisingEvents = True,
        '     .SynchronizingObject = Me
        ' }

        'MyProcess = {
        '    .StartInfo = MyStartInfo,
        '    .EnableRaisingEvents = True,
        '    .SynchronizingObject = Me
        '}


        MyProcess.StartInfo = MyStartInfo
        MyProcess.EnableRaisingEvents = True
        MyProcess.SynchronizingObject = Me

        MyProcess.Start()
        MyProcess.BeginErrorReadLine()
        MyProcess.BeginOutputReadLine()

        CurrentProcessID = MyProcess.Id

        AddHandler MyProcess.OutputDataReceived,
            Sub(sender As Object, e As DataReceivedEventArgs)
                If e.Data IsNot Nothing Then
                    BeginInvoke(New MethodInvoker(
                    Sub()
                        RichTextBox1.AppendText(e.Data + Environment.NewLine)
                        RichTextBox1.ScrollToCaret()
                    End Sub))
                End If
            End Sub

        AddHandler MyProcess.ErrorDataReceived,
            Sub(sender As Object, e As DataReceivedEventArgs)
                If e.Data IsNot Nothing Then
                    BeginInvoke(New MethodInvoker(
                    Sub()
                        RichTextBox1.AppendText(e.Data + Environment.NewLine)
                        RichTextBox1.ScrollToCaret()
                    End Sub))
                End If
            End Sub

        AddHandler MyProcess.Exited,
            Sub(source As Object, ev As EventArgs)
                MyProcess.Close()
                If MyProcess IsNot Nothing Then
                    MyProcess.Dispose()
                End If
            End Sub
    End Sub

    Private Sub BunifuThinButton212_Click(sender As Object, e As EventArgs)
        For Each proc As Process In Process.GetProcesses
            If proc.ProcessName = "FXServer" Then

                proc.Kill()
                RichTextBox1.Text = ""
            Else
                RichTextBox1.Text = ""
            End If
        Next
    End Sub


    Private Sub TextBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyDown
        For Each proc As Process In Process.GetProcesses
            If proc.ProcessName = "FXServer" Then
                If e.KeyCode = Keys.Enter Then
                    Try
                        SendCommand(TextBox2.Text)
                        TextBox2.Text = ""
                    Catch ex As Exception
                        MessageBox.Show(ex.ToString())
                    End Try
                End If
            End If
        Next
    End Sub

    Private Sub BunifuThinButton216_Click(sender As Object, e As EventArgs)
        Dim FileWriter As StreamWriter
        Dim cfg As String
        cfg = "C:\HomeWin\Servers\FiveM\server.cfg"
        If System.IO.File.Exists(cfg) Then
            FileWriter = New StreamWriter("C:\HomeWin\Servers\FiveM\server.cfg", False)
            FileWriter.Write(RichTextBox2.Text)
            FileWriter.Close()
            TabControl1.SelectedIndex = 1


        Else
            Dim dirfivem As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM")
            If dirfivem.Exists Then
                dirfivem.Delete(True)
            End If
            Me.Close()
            MsgBox("Ups! We didn't find the Server Configuration, download the server again please.", MsgBoxStyle.Critical, "Server Configuration")
        End If
    End Sub

    Private Sub BunifuThinButton213_Click(sender As Object, e As EventArgs)
        RichTextBox2.Text = ""
        TabControl1.SelectedIndex = 1
    End Sub

    Public Sub ReadConfig()
        Dim cfg As String
        cfg = "C:\HomeWin\Servers\FiveM\server.cfg"
        If System.IO.File.Exists(cfg) Then
            Dim FileReader As StreamReader
            FileReader = New StreamReader("C:\HomeWin\Servers\FiveM\server.cfg")
            RichTextBox2.Text = FileReader.ReadToEnd()
            FileReader.Close()
        Else
            Dim dirfivem As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM\resources")
            If dirfivem.Exists Then
                dirfivem.Delete(True)
            End If
            Me.Close()
            MsgBox("Ups! We didn't find the Server Configuration, download the server again please.", MsgBoxStyle.Critical, "Server Configuration")
        End If
    End Sub



    Private Sub PictureBox6_Click(sender As Object, e As EventArgs) Handles PictureBox6.Click
        TabControl1.SelectedIndex = 3
        ReadConfig()
    End Sub

    Private Sub Label13_Click(sender As Object, e As EventArgs) Handles Label13.Click
        TabControl1.SelectedIndex = 3
        ReadConfig()
    End Sub

    Public Sub clearcache()
        Dim cachefivem As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM\cache")
        If cachefivem.Exists Then
            cachefivem.Delete(True)
        Else
        End If
    End Sub

    Private Sub PictureBox10_Click(sender As Object, e As EventArgs) Handles PictureBox10.Click
        clearcache()
    End Sub

    Private Sub Label15_Click(sender As Object, e As EventArgs) Handles Label15.Click
        clearcache()
    End Sub


    Private Sub BunifuGradientPanel1_Paint(sender As Object, e As PaintEventArgs) Handles BunifuGradientPanel1.Paint

    End Sub

    Private Sub Panel3_Paint(sender As Object, e As PaintEventArgs) Handles Panel3.Paint

    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        For Each proc As Process In Process.GetProcesses
            If proc.ProcessName = "FXServer" Then
                proc.Kill()
            Else

            End If
        Next
        Application.Exit()
    End Sub

    Private Sub MaterialRaisedButton1_Click(sender As Object, e As EventArgs)
        If Backup1.Visible Then

        Else

            StartBackup.Start()
        End If
    End Sub

    Private Sub BunifuThinButton218_Click(sender As Object, e As EventArgs)
        Dim fivembackup As String = "C:\HomeWin\Servers\Backup\FiveM"
        If System.IO.Directory.Exists(fivembackup) Then
            Process.Start(fivembackup)
        End If
    End Sub

    Private Sub BunifuThinButton223_Click(sender As Object, e As EventArgs)
        Dim fivembackup As String = "C:\HomeWin\Servers\Backup\FiveM2"
        If System.IO.Directory.Exists(fivembackup) Then
            Process.Start(fivembackup)
        End If
    End Sub

    Private Sub BunifuThinButton226_Click(sender As Object, e As EventArgs)
        Dim fivembackup As String = "C:\HomeWin\Servers\Backup\FiveM3"
        If System.IO.Directory.Exists(fivembackup) Then
            Process.Start(fivembackup)
        End If
    End Sub

    Private Sub BunifuThinButton219_Click(sender As Object, e As EventArgs)
        Dim backupfivem1 As New IO.DirectoryInfo("C:\HomeWin\Servers\Backup\FiveM")
        If backupfivem1.Exists Then
            backupfivem1.Delete(True)
            Backup1.Hide()


        End If
        DateLabel.Text = "00/00/0000"
    End Sub

    Public Sub CheckBackup()
        TabControl1.SelectedIndex = 4
        Dim backupfivem As New IO.DirectoryInfo("C:\HomeWin\Servers\Backup\FiveM\resources")
        If backupfivem.Exists Then
            Backup1.Show()
        Else
            Backup1.Hide()
        End If
    End Sub


    Private Sub Label16_Click(sender As Object, e As EventArgs) Handles Label16.Click
        CheckBackup()
    End Sub

    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        CheckBackup()
    End Sub


    Private Sub StartBackup_Tick(sender As Object, e As EventArgs) Handles StartBackup.Tick
        Dim todaysdate As String = String.Format("{0:dd/MM/yyyy}", DateTime.Now)
        StartBackup.Stop()
        My.Computer.FileSystem.CreateDirectory("C:\HomeWin\Servers\Backup\FiveM")
        My.Computer.FileSystem.CopyDirectory("C:\HomeWin\Servers\FiveM", "C:\HomeWin\Servers\Backup\FiveM", True)
        DateLabel.Text = todaysdate
        Backup1.Show()
        PictureBox12.Hide()
    End Sub

    Private Sub BunifuThinButton220_Click(sender As Object, e As EventArgs)
        ReplaceServer.ShowDialog()
    End Sub

    Private Sub CheckServerExist_Tick(sender As Object, e As EventArgs) Handles CheckServerExist.Tick
        Dim fivemfolder As String = "C:\HomeWin\Servers\FiveM\resources"
        If System.IO.Directory.Exists(fivemfolder) Then

        Else
            GetDiskUsage.Stop()
            MsgBox("FiveM Server not found! Closing application...")
            Application.Exit()
        End If
    End Sub

    'Function MD5
    Function MDhash(ByVal password As String)
        Dim md5 As MD5 = New MD5CryptoServiceProvider()
        Dim result As Byte()
        result = md5.ComputeHash(Encoding.ASCII.GetBytes(password))
        Dim strBuilder As New StringBuilder()
        For i As Integer = 0 To result.Length - 1
            strBuilder.Append(result(i).ToString("x2"))
        Next
        Return strBuilder.ToString()
    End Function

    Private Sub BunifuThinButton217_Click(sender As Object, e As EventArgs)
        'Efetuar Login
        Dim value As String = MDhash(MaterialSingleLineTextField2.Text)
        Username.Text = MaterialSingleLineTextField1.Text
        Password.Text = value
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("https://drive.google.com/uc?authuser=0&id=1CEc1XtYVqX4ma7iwTFR_WDXNv1N8brwZ&export=download")
        Dim response As System.Net.HttpWebResponse = request.GetResponse()
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())
        Dim newestversion As String = sr.ReadToEnd()
        Dim connection As New MySqlConnection(newestversion)

        Dim command As New MySqlCommand("SELECT * FROM `u115781387_members` WHERE `email` = @email AND `password` = @password", connection)


        command.Parameters.Add("@email", MySqlDbType.VarChar).Value = Username.Text
        command.Parameters.Add("@password", MySqlDbType.VarChar).Value = Password.Text

        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()
        adapter.Fill(table)
        If table.Rows.Count = 0 Then
            'Couldn't find any acc
            Label39.Show()
            Label38.Show()
            Username.Text = ""
            Password.Text = ""
        Else
            Label39.Hide()
            Label38.Hide()
            'Finded acc
            Label18.Text = "Account Details"
            TabControl1.SelectedIndex = 1
            My.Settings.Save()

            Dim StatusValue As String = table(0)(6)
            Dim PremiumValue As String = table(0)(26)
            If StatusValue = "active" Then
                If PremiumValue = 1 Then
                    'Se for premium
                    Label20.Text = "Active"

                    Label19.Hide()
                    PictureBox4.Hide()
                    Label20.ForeColor = Color.SeaGreen
                    PremiumPanel.Show()


                Else
                    'Se nao for premium
                    Label20.Text = "Inactive"
                    Label20.ForeColor = Color.Crimson
                    PremiumPanel.Hide()


                End If
            Else
                MsgBox("Please go to your e-mail and confirm your account. Don't forget to check SPAM/TRASH.", MsgBoxStyle.Information, "Account confirmation")
            End If
        End If
    End Sub


    Public Sub CheckForLogins()
        'Efetuar Login
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("https://drive.google.com/uc?authuser=0&id=1CEc1XtYVqX4ma7iwTFR_WDXNv1N8brwZ&export=download")
        Dim response As System.Net.HttpWebResponse = request.GetResponse()
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())
        Dim newestversion As String = sr.ReadToEnd()
        Dim connection As New MySqlConnection(newestversion)

        Dim command As New MySqlCommand("SELECT * FROM `u115781387_members` WHERE `email` = @email AND `password` = @password", connection)


        command.Parameters.Add("@email", MySqlDbType.VarChar).Value = Username.Text
        command.Parameters.Add("@password", MySqlDbType.VarChar).Value = Password.Text

        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()
        adapter.Fill(table)
        If table.Rows.Count = 0 Then
            'Couldn't find any acc
            TabControl1.SelectedIndex = 1

        Else
            'Finded acc
            TabControl1.SelectedIndex = 1
            Dim StatusValue As String = table(0)(6)
            Label18.Text = "Account Details"
            Dim PremiumValue As String = table(0)(26)
            If StatusValue = "active" Then
                If PremiumValue = 1 Then
                    'Se for premium
                    Label20.Text = "Active"
                    Label19.Hide()
                    PictureBox4.Hide()
                    Label20.ForeColor = Color.SeaGreen
                    PremiumPanel.Show()


                Else
                    'Se nao for premium
                    Panel17.Show()
                    Label20.Text = "Inactive"
                    Label20.ForeColor = Color.Crimson
                    PremiumPanel.Hide()


                End If
            Else
                MsgBox("Please go to your e-mail and confirm your account. Don't forget to check SPAM/TRASH.", MsgBoxStyle.Information, "Account confirmation")
            End If
        End If
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Process.Start(dirPath + "\Updater.exe")
        Application.Exit()
    End Sub

    Private Sub PictureBox20_Click(sender As Object, e As EventArgs) Handles PictureBox20.Click
        ShowESX()
    End Sub

    Private Sub TabPage7_Click(sender As Object, e As EventArgs) Handles TabPage7.Click

    End Sub

    Private Sub MaterialTabSelector1_Click(sender As Object, e As EventArgs)
        If MaterialTabControl1.SelectedIndex = 2 Then
            Dim esxpath As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM\resources\[esx]")
            If esxpath.Exists Then
                TabControl2.SelectedIndex = 4
                MaterialTabControl1.SelectedIndex = 0
            Else
                TabControl2.SelectedIndex = 0
                MaterialTabControl1.SelectedIndex = 2
                Timer1.Start()
            End If
        ElseIf MaterialTabControl1.SelectedIndex = 0 Then
            Dim esxpath As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM\resources\[esx]")
            If esxpath.Exists Then
                'do nothing
            Else
                TabControl2.SelectedIndex = 0
                MaterialTabControl1.SelectedIndex = 2
                Timer1.Start()
            End If
        ElseIf MaterialTabControl1.SelectedIndex = 1 Then
            Dim esxpath As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM\resources\[esx]")
            If esxpath.Exists Then
                'do nothing
            Else
                TabControl2.SelectedIndex = 0
                MaterialTabControl1.SelectedIndex = 2
                Timer1.Start()
            End If


        End If
    End Sub

    Private Sub BunifuThinButton221_Click(sender As Object, e As EventArgs)
        Process.Start("https://www.heidisql.com/installers/HeidiSQL_10.1.0.5464_Setup.exe")
    End Sub

    Private Sub BunifuThinButton227_Click(sender As Object, e As EventArgs)
        Process.Start("https://downloadsapachefriends.global.ssl.fastly.net/7.3.3/xampp-windows-x64-7.3.3-1-VC15-installer.exe?from_af=true")
    End Sub

    Private Sub BunifuThinButton229_Click(sender As Object, e As EventArgs)
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub BunifuThinButton230_Click(sender As Object, e As EventArgs)
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub BunifuThinButton232_Click(sender As Object, e As EventArgs)
        TabControl2.SelectedIndex = 1
    End Sub

    Private Sub BunifuThinButton231_Click(sender As Object, e As EventArgs)
        If FlatComboBox1.Text = "" Then
            MsgBox("You need to choose a language for ESX.", MsgBoxStyle.Information, "Choose Language")
        ElseIf FlatComboBox1.Text = "English" Then
            'Se a linguagem for ingles
            TabControl2.SelectedIndex = 3
            Dim esxpath As String = "C:\HomeWin\Servers\FiveM\resources\"
            AddHandler wClient.DownloadProgressChanged, AddressOf ProgChanged
            wClient.DownloadFileAsync(New Uri("https://cdn.discordapp.com/attachments/372105249672134656/583991181109886986/esx.zip"), esxpath + "\esx.zip")
            Timer5.Start()

        ElseIf FlatComboBox1.Text = "Português" Then
            'Se a linguagem for Portugues
            TabControl2.SelectedIndex = 3
            Dim esxpath As String = "C:\HomeWin\Servers\FiveM\resources\"
            AddHandler wClient.DownloadProgressChanged, AddressOf ProgChanged
            wClient.DownloadFileAsync(New Uri("https://cdn.discordapp.com/attachments/372105249672134656/583987012282089472/esx.zip"), esxpath + "\esx.zip")
            Timer5.Start()

        ElseIf FlatComboBox1.Text = "Français" Then
            'Se a linguagem for Frances
            TabControl2.SelectedIndex = 3
            Dim esxpath As String = "C:\HomeWin\Servers\FiveM\resources\"
            AddHandler wClient.DownloadProgressChanged, AddressOf ProgChanged
            wClient.DownloadFileAsync(New Uri("https://cdn.discordapp.com/attachments/372105249672134656/583989778526109706/esx.zip"), esxpath + "\esx.zip")
            Timer5.Start()

        End If
    End Sub

    Private Sub ProgChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        ProgressBar1.Value = e.ProgressPercentage
        ProgressBar2.Value = e.ProgressPercentage
        BunifuProgressBar1.Value = e.ProgressPercentage
    End Sub

    Private Sub MaterialRaisedButton2_Click(sender As Object, e As EventArgs)
        Process.Start("https://www.youtube.com/watch?v=xFqT4WqAV0Q")
    End Sub

    Private Sub BunifuFlatButton4_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton4.Click
        My.Computer.Clipboard.SetText(RichTextBox3.Text)
        TabControl1.SelectedIndex = 3
        TabControl2.SelectedIndex = 0
        ReadConfig()
    End Sub

    Public Sub ShowESX()
        TabControl1.SelectedIndex = 6
        WebBrowser1.Navigate("https://sgproleplaypt.wixsite.com/homewin/scripts-2")
        Dim esxpath As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM\resources\[esx]")
        If esxpath.Exists Then
            TabControl2.SelectedIndex = 4
            MaterialTabControl1.SelectedIndex = 2
        Else
            TabControl2.SelectedIndex = 0
            MaterialTabControl1.SelectedIndex = 2
            Timer1.Start()
        End If
    End Sub

    Public Sub ShowVRP()
        TabControl1.SelectedIndex = 7
        WebBrowser2.Navigate("https://sgproleplaypt.wixsite.com/homewin/vrpscripts")
        Dim vrppath As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM\resources\[vrp]")
        If vrppath.Exists Then
            TabControl3.SelectedIndex = 3
            MaterialTabControl2.SelectedIndex = 2
        Else
            TabControl3.SelectedIndex = 1
            MaterialTabControl2.SelectedIndex = 2
        End If
    End Sub

    Private Sub Label37_Click(sender As Object, e As EventArgs) Handles Label37.Click
        ShowESX()
    End Sub

    Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        TabControl2.SelectedIndex = 1
    End Sub

    Private Sub MaterialRaisedButton4_Click(sender As Object, e As EventArgs)
        System.IO.Directory.Delete("C:\HomeWin\Servers\FiveM\resources\[esx]", True)
        TabControl2.SelectedIndex = 0
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub BunifuThinButton228_Click(sender As Object, e As EventArgs)
        TabControl2.SelectedIndex = 2
    End Sub

    Private Sub Timer5_Tick(sender As Object, e As EventArgs) Handles Timer5.Tick
        If (ProgressBar2.Value = 100) Then
            Timer5.Stop()
            ProgressBar2.Value = 0
            Timer6.Start()
        Else
            'Nothing
        End If
    End Sub

    Private Sub Timer6_Tick(sender As Object, e As EventArgs) Handles Timer6.Tick
        Timer6.Stop()
        Dim extractpath As String = "C:\HomeWin\Servers\FiveM\resources"
        Dim Zippath As String = "C:\HomeWin\Servers\FiveM\resources\esx.zip"
        ZipFile.ExtractToDirectory(Zippath, extractpath)
        My.Computer.FileSystem.DeleteFile("C:\HomeWin\Servers\FiveM\resources\esx.zip")
        TabControl2.SelectedIndex = 4
    End Sub

    Private Sub MaterialRaisedButton3_Click(sender As Object, e As EventArgs)
        MaterialTabControl1.SelectedIndex = 0
    End Sub


    Private Sub PictureBox15_Click(sender As Object, e As EventArgs) Handles PictureBox15.Click
        Process.Start("https://discord.gg/2KE4dgp")
    End Sub

    Private Sub Label33_Click(sender As Object, e As EventArgs) Handles Label33.Click
        Process.Start("https://discord.gg/2KE4dgp")
    End Sub

    Private Sub BunifuFlatButton1_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton1.Click
        My.Computer.Clipboard.SetText(RichTextBox4.Text)
        TabControl1.SelectedIndex = 3
        TabControl2.SelectedIndex = 0
        ReadConfig()
    End Sub

    Private Sub BunifuThinButton236_Click(sender As Object, e As EventArgs)
        TabControl3.SelectedIndex = 2
        Dim vrppath As String = "C:\HomeWin\Servers\FiveM\resources\"
        AddHandler wClient.DownloadProgressChanged, AddressOf ProgChanged
        wClient.DownloadFileAsync(New Uri("https://ia601403.us.archive.org/10/items/rubenribeiroelxaister2001_gmail_Vrp/%5Bvrp%5D.zip"), vrppath + "\vrp.zip")
        Timer4.Start()
    End Sub



    Private Sub BunifuFlatButton2_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton2.Click
        My.Computer.Clipboard.SetText(RichTextBox5.Text)
        TabControl1.SelectedIndex = 3
        TabControl2.SelectedIndex = 0
        ReadConfig()
    End Sub


    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        Timer3.Stop()
        My.Computer.FileSystem.CreateDirectory("C:\HomeWin\Servers\FiveM\")
        AddHandler wClient.DownloadProgressChanged, AddressOf ProgChanged
        wClient.DownloadFileAsync(New Uri("https://ia601401.us.archive.org/27/items/FiveM/FiveM.zip"), fivempath + "\server.zip")
        Timer9.Start()
    End Sub

    Private Sub PictureBox19_Click(sender As Object, e As EventArgs) Handles PictureBox19.Click
        ShowVRP()
    End Sub


    Private Sub Label36_Click(sender As Object, e As EventArgs) Handles Label36.Click
        ShowVRP()
    End Sub

    Private Sub MaterialRaisedButton5_Click(sender As Object, e As EventArgs)
        System.IO.Directory.Delete("C:\HomeWin\Servers\FiveM\resources\[vrp]", True)
        TabControl3.SelectedIndex = 1
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick
        If (ProgressBar1.Value = 100) Then
            Timer4.Stop()
            TabControl3.SelectedIndex = 3
            ProgressBar1.Value = 0
            Timer7.Start()
        Else
            'Nothing
        End If
    End Sub

    Private Sub Timer7_Tick(sender As Object, e As EventArgs) Handles Timer7.Tick
        Timer7.Stop()
        Dim extractpath As String = "C:\HomeWin\Servers\FiveM\resources"
        Dim Zippath As String = "C:\HomeWin\Servers\FiveM\resources\vrp.zip"
        ZipFile.ExtractToDirectory(Zippath, extractpath)
        My.Computer.FileSystem.DeleteFile("C:\HomeWin\Servers\FiveM\resources\vrp.zip")
        TabControl3.SelectedIndex = 4
    End Sub

    Private Sub MaterialRaisedButton6_Click(sender As Object, e As EventArgs)
        MaterialTabControl2.SelectedIndex = 0
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub MaterialRaisedButton7_Click(sender As Object, e As EventArgs)
        Process.Start("https://www.youtube.com/watch?v=Vpa3RTMVRZQ")
    End Sub

    Private Sub BunifuFlatButton3_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton3.Click
        System.IO.Directory.Delete("C:\HomeWin\Servers\FiveM\resources\[vrp]", True)
        TabControl3.SelectedIndex = 1
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub BunifuFlatButton5_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton5.Click
        Process.Start("https://www.youtube.com/watch?v=Vpa3RTMVRZQ")
    End Sub

    Private Sub BunifuFlatButton6_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton6.Click
        TabControl1.SelectedIndex = 1
        WebBrowser2.Stop()
    End Sub

    Private Sub BunifuFlatButton7_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton7.Click
        System.IO.Directory.Delete("C:\HomeWin\Servers\FiveM\resources\[esx]", True)
        TabControl2.SelectedIndex = 0
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub BunifuFlatButton8_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton8.Click
        Process.Start("https://www.youtube.com/watch?v=xFqT4WqAV0Q")
    End Sub

    Private Sub BunifuFlatButton9_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton9.Click
        TabControl1.SelectedIndex = 1
        WebBrowser1.Stop()
    End Sub

    Public Sub CheckLogin()
        If Username.Text = "" Then
            TabControl1.SelectedIndex = 5
        Else
            TabControl1.SelectedIndex = 9
            If Label20.Text = "Active" Then
                Label58.Show()
                Label59.Show()
                Label60.Text = "PREMIUM ACTIVE"
            Else
                Label60.Text = "PREMIUM INACTIVE"
            End If
            Label64.Text = Username.Text
        End If
    End Sub

    Private Sub PictureBox9_Click(sender As Object, e As EventArgs) Handles PictureBox9.Click
        CheckLogin()
    End Sub

    Private Sub Label19_Click(sender As Object, e As EventArgs) Handles Label19.Click
        TabControl1.SelectedIndex = 8
    End Sub

    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click
        TabControl1.SelectedIndex = 8
    End Sub


    Private Sub Label18_Click(sender As Object, e As EventArgs) Handles Label18.Click
        CheckLogin()
    End Sub


    Private Sub MaterialLabel15_Click(sender As Object, e As EventArgs) Handles MaterialLabel15.Click

    End Sub

    Private Sub Label42_Click(sender As Object, e As EventArgs) Handles Label42.Click
        Process.Start("https://keymaster.fivem.net/")
    End Sub

    Private Sub MaterialRaisedButton2_Click_1(sender As Object, e As EventArgs) Handles MaterialRaisedButton2.Click
        If ServerName.Text = "" Then
            MsgBox("Please give the server a name.", MsgBoxStyle.Critical, "Server Name")
        Else
            If ServerSlot.Text = "" Then
                MsgBox("Please choose the number of slots on your server.", MsgBoxStyle.Critical, "Server Slots")
            Else

                If ServerList.Text = "" Then
                    MsgBox("Please choose whether you want the server in the list.", MsgBoxStyle.Critical, "Server List")

                Else
                    If ServerKey.Text = "" Then
                        MsgBox("You have to put a key. Please visit https://keymaster.fivem.net/", MsgBoxStyle.Critical, "FiveM Key")

                    Else
                        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("https://drive.google.com/uc?authuser=0&id=1sAaOnaS-XZ4cUDyc1wtfR9ff9c5MEuNg&export=download")
                        Dim aspas As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("https://drive.google.com/uc?authuser=0&id=1mVIBMn-ju349HF_PKzkKu17q_QoBRwZu&export=download")
                        Dim response1 As System.Net.HttpWebResponse = aspas.GetResponse()
                        Dim response As System.Net.HttpWebResponse = request.GetResponse()
                        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())
                        Dim sr1 As System.IO.StreamReader = New System.IO.StreamReader(response1.GetResponseStream())
                        Dim aspasfile As String = sr1.ReadToEnd()
                        Dim configfile As String = sr.ReadToEnd()
                        Dim file As System.IO.StreamWriter
                        file = My.Computer.FileSystem.OpenTextFileWriter("C:\HomeWin\Servers\FiveM\server.cfg", True)
                        file.WriteLine(configfile)
                        file.WriteLine("")
                        file.WriteLine("sv_hostname " + aspasfile + ServerName.Text + aspasfile)
                        file.WriteLine("")
                        If ServerList.Text = "Yes" Then
                            file.WriteLine("#sv_master1 " + aspasfile + aspasfile)
                        Else
                            file.WriteLine("sv_master1 " + aspasfile + aspasfile)
                        End If
                        file.WriteLine("")
                        file.WriteLine("sv_maxclients " + ServerSlot.Text)
                        file.WriteLine("")
                        file.WriteLine("sv_licenseKey " + aspasfile + ServerKey.Text + aspasfile)
                        file.Close()
                        getpublicip()
                        GetDiskUsage.Start()
                        CheckForLogins()
                        GetServerStatus.Start()
                        GetDiskUsage.Interval = 2500
                        GetServerStatus.Start()
                        CheckServerExist.Start()
                        TabControl1.SelectedIndex = 8
                    End If

                End If

            End If

        End If
    End Sub

    Private Sub Timer8_Tick(sender As Object, e As EventArgs) Handles Timer8.Tick
        Timer8.Stop()
        TabControl3.SelectedIndex = 1
    End Sub

    Private Sub Timer9_Tick(sender As Object, e As EventArgs) Handles Timer9.Tick
        If (BunifuProgressBar1.Value = 100) Then
            Timer9.Stop()
            Timer10.Start()
            BunifuProgressBar1.Value = 0
            ProgressBar1.Value = 0
            ProgressBar2.Value = 0
        Else
            'Nothing
        End If
    End Sub

    Private Sub Timer10_Tick(sender As Object, e As EventArgs) Handles Timer10.Tick
        Timer10.Stop()
        Dim extractpath As String = "C:\HomeWin\Servers\FiveM\"
        Dim Zippath As String = "C:\HomeWin\Servers\FiveM\server.zip"
        ZipFile.ExtractToDirectory(Zippath, extractpath)
        My.Computer.FileSystem.DeleteFile("C:\HomeWin\Servers\FiveM\server.zip")
        Panel9.Hide()
        Panel8.Show()
    End Sub

    Private Sub BunifuFlatButton10_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton10.Click
        Process.Start("https://infinyctus.com/en/")
    End Sub

    Private Sub BunifuFlatButton11_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton11.Click
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub MaterialRaisedButton3_Click_1(sender As Object, e As EventArgs) Handles MaterialRaisedButton3.Click
        TabControl1.SelectedIndex = 1
    End Sub

    Public Sub ShowScripts()
        TabControl1.SelectedIndex = 11
        WebBrowser3.Navigate("https://sgproleplaypt.wixsite.com/homewin/scripts")
    End Sub

    Public Sub ShowModels()
        TabControl1.SelectedIndex = 12
    End Sub

    Private Sub Label35_Click(sender As Object, e As EventArgs) Handles Label35.Click
        ShowScripts()
    End Sub



    Private Sub PictureBox18_Click(sender As Object, e As EventArgs) Handles PictureBox18.Click
        ShowScripts()
    End Sub

    Private Sub Label68_Click(sender As Object, e As EventArgs) Handles Label72.Click
        Dim vehiclelink As String = "https://drive.google.com/uc?authuser=0&id=16e1pPVW_ldFmwOPRGKFcgh9zkOKLuXdL&export=download"
        Process.Start(vehiclelink)
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label71.Click
        Dim vehiclelink As String = "https://drive.google.com/uc?authuser=0&id=1l8HLx6Mutj-oHij_NlSJPuQXexkLbLet&export=download"
        Process.Start(vehiclelink)
    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label70.Click
        Dim vehiclelink As String = "https://ia801507.us.archive.org/28/items/realistic_carpack/realistic_carpack.zip"
        Process.Start(vehiclelink)
    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label69.Click
        Dim vehiclelink As String = "https://drive.google.com/uc?authuser=0&id=1p9WMt7akiTkJ-joJ2LOhAk5a2KSzBfrx&export=download"
        Process.Start(vehiclelink)
    End Sub


    Private Sub Label7_Click(sender As Object, e As EventArgs) Handles Label68.Click
        Dim vehiclelink As String = "https://drive.google.com/uc?authuser=0&id=1kgtxXBV8wDb0vLuySj4dPk95s50H_xLi&export=download"
        Process.Start(vehiclelink)
    End Sub


    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label67.Click
        Dim vehiclelink As String = "https://drive.google.com/uc?authuser=0&id=1GdF6mkunjFuMxx4cZZTP2i8529zn_AO0&export=download"
        Process.Start(vehiclelink)
    End Sub

    Private Sub PictureBox17_Click(sender As Object, e As EventArgs) Handles PictureBox17.Click
        ShowModels()
    End Sub


    Private Sub Label30_Click(sender As Object, e As EventArgs) Handles Label30.Click
        ShowModels()
    End Sub

    Private Sub BunifuThinButton238_Click(sender As Object, e As EventArgs)
        Process.Start("https://www.heidisql.com/installers/HeidiSQL_10.1.0.5464_Setup.exe")
    End Sub

    Private Sub BunifuThinButton237_Click(sender As Object, e As EventArgs)
        Process.Start("https://downloadsapachefriends.global.ssl.fastly.net/7.3.3/xampp-windows-x64-7.3.3-1-VC15-installer.exe?from_af=true")
    End Sub

    Private Sub BunifuThinButton235_Click(sender As Object, e As EventArgs)
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub BunifuFlatButton12_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton12.Click
        MaterialTabControl1.SelectedIndex = 0
    End Sub

    Private Sub BunifuFlatButton13_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton13.Click
        MaterialTabControl1.SelectedIndex = 1
    End Sub

    Private Sub BunifuFlatButton15_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton15.Click
        MaterialTabControl2.SelectedIndex = 0
    End Sub

    Private Sub BunifuFlatButton14_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton14.Click
        MaterialTabControl2.SelectedIndex = 1
    End Sub

    Private Sub Label74_Click(sender As Object, e As EventArgs) Handles Label74.Click

    End Sub

    Private Sub Label73_Click(sender As Object, e As EventArgs) Handles Label73.Click

    End Sub

    Private Sub MaterialRaisedButton4_Click_1(sender As Object, e As EventArgs) Handles MaterialRaisedButton4.Click
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub MaterialRaisedButton5_Click_1(sender As Object, e As EventArgs) Handles MaterialRaisedButton5.Click
        If Backup1.Visible Then
            PictureBox12.Hide()
        Else
            PictureBox12.Show()
            StartBackup.Start()
        End If
    End Sub

    Private Sub MaterialRaisedButton1_Click_1(sender As Object, e As EventArgs) Handles MaterialRaisedButton1.Click
        TabControl1.SelectedIndex = 1
        Label20.Text = "Inactive"
        PremiumPanel.Hide()
        Label19.Show()
        PictureBox4.Show()
        Username.Text = ""
        Password.Text = ""
        My.Settings.Reset()
    End Sub

    Private Sub Label27_Click(sender As Object, e As EventArgs) Handles Label27.Click
        Process.Start("https://infinyctus.com/en/")
    End Sub


    Private Sub MaterialRaisedButton6_Click_1(sender As Object, e As EventArgs) Handles MaterialRaisedButton6.Click
        Dim p() As Process
        p = Process.GetProcessesByName("FXServer")
        If p.Count > 0 Then

        Else
            For Each proc As Process In Process.GetProcesses
                If proc.ProcessName = "FXServer" Then

                Else
                    StartServerProcess.Start()


                End If
            Next
        End If
    End Sub

    Private Sub MaterialRaisedButton7_Click_1(sender As Object, e As EventArgs) Handles MaterialRaisedButton7.Click
        For Each proc As Process In Process.GetProcesses
            If proc.ProcessName = "FXServer" Then
                proc.Kill()
                RichTextBox1.Text = ""
                Timer11.Start()
            End If
        Next

    End Sub

    Private Sub MaterialRaisedButton8_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton8.Click
        For Each proc As Process In Process.GetProcesses
            If proc.ProcessName = "FXServer" Then

                proc.Kill()
                CheckPlayerList.Stop()
                RichTextBox1.Text = ""
            Else
                RichTextBox1.Text = ""
            End If
        Next
    End Sub

    Private Sub MaterialRaisedButton9_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton9.Click
        Dim FileWriter As StreamWriter
        Dim cfg As String
        cfg = "C:\HomeWin\Servers\FiveM\server.cfg"
        If System.IO.File.Exists(cfg) Then
            FileWriter = New StreamWriter("C:\HomeWin\Servers\FiveM\server.cfg", False)
            FileWriter.Write(RichTextBox2.Text)
            FileWriter.Close()
            TabControl1.SelectedIndex = 1


        Else
            Dim dirfivem As New IO.DirectoryInfo("C:\HomeWin\Servers\FiveM")
            If dirfivem.Exists Then
                dirfivem.Delete(True)
            End If
            Me.Close()
            MsgBox("Ups! We didn't find the Server Configuration, download the server again please.", MsgBoxStyle.Critical, "Server Configuration")
        End If
    End Sub

    Private Sub MaterialRaisedButton10_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton10.Click
        RichTextBox2.Text = ""
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub MaterialRaisedButton11_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton11.Click
        Dim fivembackup As String = "C:\HomeWin\Servers\Backup\FiveM"
        If System.IO.Directory.Exists(fivembackup) Then
            Process.Start(fivembackup)
        End If
    End Sub

    Private Sub MaterialRaisedButton12_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton12.Click
        Dim backupfivem1 As New IO.DirectoryInfo("C:\HomeWin\Servers\Backup\FiveM")
        If backupfivem1.Exists Then
            backupfivem1.Delete(True)
            Backup1.Hide()


        End If
        DateLabel.Text = "00/00/0000"
    End Sub

    Private Sub MaterialRaisedButton13_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton13.Click
        'Efetuar Login
        Dim value As String = MDhash(MaterialSingleLineTextField2.Text)
        Username.Text = MaterialSingleLineTextField1.Text
        Password.Text = value
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("https://drive.google.com/uc?authuser=0&id=1CEc1XtYVqX4ma7iwTFR_WDXNv1N8brwZ&export=download")
        Dim response As System.Net.HttpWebResponse = request.GetResponse()
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())
        Dim newestversion As String = sr.ReadToEnd()
        Dim connection As New MySqlConnection(newestversion)

        Dim command As New MySqlCommand("SELECT * FROM `u115781387_members` WHERE `email` = @email AND `password` = @password", connection)


        command.Parameters.Add("@email", MySqlDbType.VarChar).Value = Username.Text
        command.Parameters.Add("@password", MySqlDbType.VarChar).Value = Password.Text

        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()
        adapter.Fill(table)
        If table.Rows.Count = 0 Then
            'Couldn't find any acc
            Label39.Show()
            Label38.Show()
            Username.Text = ""
            Password.Text = ""
        Else
            Label39.Hide()
            Label38.Hide()
            'Finded acc
            Label18.Text = "Account Details"
            TabControl1.SelectedIndex = 1
            My.Settings.Save()

            Dim StatusValue As String = table(0)(6)
            Dim PremiumValue As String = table(0)(26)
            If StatusValue = "active" Then
                If PremiumValue = 1 Then
                    'Se for premium
                    Label20.Text = "Active"

                    Label19.Hide()
                    PictureBox4.Hide()
                    Label20.ForeColor = Color.SeaGreen
                    PremiumPanel.Show()


                Else
                    'Se nao for premium
                    Label20.Text = "Inactive"
                    Label20.ForeColor = Color.Crimson
                    PremiumPanel.Hide()


                End If
            Else
                MsgBox("Please go to your e-mail and confirm your account. Don't forget to check SPAM/TRASH.", MsgBoxStyle.Information, "Account confirmation")
            End If
        End If
    End Sub

    Private Sub MaterialRaisedButton14_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton14.Click
        Process.Start("https://www.heidisql.com/installers/HeidiSQL_10.1.0.5464_Setup.exe")
    End Sub

    Private Sub MaterialRaisedButton15_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton15.Click
        Process.Start("https://downloadsapachefriends.global.ssl.fastly.net/7.3.3/xampp-windows-x64-7.3.3-1-VC15-installer.exe?from_af=true")
    End Sub

    Private Sub MaterialRaisedButton17_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton17.Click
        TabControl2.SelectedIndex = 2
    End Sub

    Private Sub MaterialRaisedButton16_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton16.Click
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub MaterialRaisedButton19_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton19.Click
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub MaterialRaisedButton18_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton18.Click
        TabControl2.SelectedIndex = 1
    End Sub

    Private Sub MaterialRaisedButton20_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton20.Click
        If FlatComboBox1.Text = "" Then
            MsgBox("You need to choose a language for ESX.", MsgBoxStyle.Information, "Choose Language")
        ElseIf FlatComboBox1.Text = "English" Then
            'Se a linguagem for ingles
            TabControl2.SelectedIndex = 3
            Dim esxpath As String = "C:\HomeWin\Servers\FiveM\resources\"
            AddHandler wClient.DownloadProgressChanged, AddressOf ProgChanged
            wClient.DownloadFileAsync(New Uri("https://cdn.discordapp.com/attachments/372105249672134656/583991181109886986/esx.zip"), esxpath + "\esx.zip")
            Timer5.Start()

        ElseIf FlatComboBox1.Text = "Português" Then
            'Se a linguagem for Portugues
            TabControl2.SelectedIndex = 3
            Dim esxpath As String = "C:\HomeWin\Servers\FiveM\resources\"
            AddHandler wClient.DownloadProgressChanged, AddressOf ProgChanged
            wClient.DownloadFileAsync(New Uri("https://cdn.discordapp.com/attachments/372105249672134656/583987012282089472/esx.zip"), esxpath + "\esx.zip")
            Timer5.Start()

        ElseIf FlatComboBox1.Text = "Français" Then
            'Se a linguagem for Frances
            TabControl2.SelectedIndex = 3
            Dim esxpath As String = "C:\HomeWin\Servers\FiveM\resources\"
            AddHandler wClient.DownloadProgressChanged, AddressOf ProgChanged
            wClient.DownloadFileAsync(New Uri("https://cdn.discordapp.com/attachments/372105249672134656/583989778526109706/esx.zip"), esxpath + "\esx.zip")
            Timer5.Start()

        End If
    End Sub

    Private Sub MaterialRaisedButton22_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton22.Click
        Process.Start("https://downloadsapachefriends.global.ssl.fastly.net/7.3.3/xampp-windows-x64-7.3.3-1-VC15-installer.exe?from_af=true")
    End Sub

    Private Sub MaterialRaisedButton21_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton21.Click
        Process.Start("https://www.heidisql.com/installers/HeidiSQL_10.1.0.5464_Setup.exe")
    End Sub

    Private Sub MaterialRaisedButton23_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton23.Click
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub MaterialRaisedButton24_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton24.Click
        TabControl3.SelectedIndex = 2
        Dim vrppath As String = "C:\HomeWin\Servers\FiveM\resources\"
        AddHandler wClient.DownloadProgressChanged, AddressOf ProgChanged
        wClient.DownloadFileAsync(New Uri("https://ia601403.us.archive.org/10/items/rubenribeiroelxaister2001_gmail_Vrp/%5Bvrp%5D.zip"), vrppath + "\vrp.zip")
        Timer4.Start()
    End Sub

    Private Sub Timer11_Tick(sender As Object, e As EventArgs) Handles Timer11.Tick
        Timer11.Stop()
        StartProcess("C:\HomeWin\Servers\FiveM\startserver.cmd", "")
    End Sub

    Private Async Function getplayerlist() As Task(Of String)
        Dim p_request As WebRequest
        Dim p_response As WebResponse
        Dim sr_stream As StreamReader

        p_request = WebRequest.Create("http://localhost:30120/players.json")
        p_response = Await p_request.GetResponseAsync()

        sr_stream = New StreamReader(p_response.GetResponseStream())

        Dim val = Await sr_stream.ReadToEndAsync()

        sr_stream.Close()

        Return val
    End Function

    Private Function returnPlayerNames(val As String) As List(Of PlayerClass)
        Dim ex As String = """name"":""(.*?)"",""ping"":(.*?)}" ''"name":"(.*?)","ping":(.*?)}
        Dim mc As MatchCollection = Regex.Matches(val, ex)
        Dim ma As Match
        Dim data As List(Of String) = New List(Of String)
        Dim pclass As List(Of PlayerClass) = New List(Of PlayerClass)

        For Each ma In mc
            Dim player As PlayerClass = New PlayerClass With {
                .Name = ma.Groups(1).Value,
                .Ping = ma.Groups(2).Value
            }

            pclass.Add(player)
        Next

        Return pclass
    End Function

    Private Async Sub CheckPlayerList_Tick(sender As Object, e As EventArgs) Handles CheckPlayerList.Tick
        Dim plist As List(Of PlayerClass) = returnPlayerNames(Await getplayerlist())
        Dim ar(2) As String
        Dim item As ListViewItem
        ListView1.Items.Clear()

        For Each ply In plist
            ar(0) = ply.Name
            ar(1) = ply.Ping
            item = New ListViewItem(ar)


            ListView1.Items.Insert(0, item)
        Next


    End Sub

    Private Sub ListView1_MouseClick(sender As Object, e As MouseEventArgs) Handles ListView1.MouseClick
        If e.Button = MouseButtons.Right Then
            If (ListView1.FocusedItem.Bounds.Contains(e.Location)) Then
                FlatContextMenuStrip1.Show(Cursor.Position)
            End If
        End If
    End Sub

    Private Sub FlatContextMenuStrip1_MouseClick(sender As Object, e As MouseEventArgs) Handles FlatContextMenuStrip1.MouseClick
        Dim item As ListViewItem = ListView1.FocusedItem

        Dim sr As StreamWriter = MyProcess.StandardInput

        Dim reason As String = InputBox("Reason?", "Kicking user: " + item.Text, "", 0, 0)

        If (reason IsNot Nothing) Then
            sr.WriteLine("clientkick " + item.Text + reason)
        Else
            MessageBox.Show("You did not give a reson!")
        End If

    End Sub

End Class
