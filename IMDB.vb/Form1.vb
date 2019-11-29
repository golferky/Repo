'Public Class Form1

'End Class

Imports System.Net
Imports System.IO

Public Class Form1
    Private moviedetails As String = Nothing
    Private searchdir As String = Nothing
    Private savedir As String = Nothing
    Private errorlist As String = Nothing
    Private successful As Integer = 0
    Private total As Integer = 0

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles SelectSource.Click
        Dim fb As New FolderBrowserDialog
        fb.Description = "Select the Folder"
        fb.RootFolder = Environment.SpecialFolder.MyComputer
        Dim dlgResult As DialogResult = fb.ShowDialog()

        If dlgResult = Windows.Forms.DialogResult.OK Then
            searchdir = fb.SelectedPath
            Dim di As New DirectoryInfo(searchdir)
            Dim fiArr As FileInfo() = di.GetFiles()
            NumberOfMovies.Text = "Number of movies in directory: " & fiArr.Count
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles StartScraping.Click
        total = 0
        successful = 0
        moviedetails = Nothing
        WriteConsolus("Initiating connection to database.")
        If searchdir IsNot Nothing And savedir IsNot Nothing Then
            Dim di As New DirectoryInfo(searchdir)
            Dim fiArr As FileInfo() = di.GetFiles()
            Dim fri As FileInfo
            Dim classif As String = Nothing
            Dim time As String = Nothing
            Dim genre As String = Nothing
            Dim plot As String = Nothing
            Dim rating As String = Nothing
            Dim mtitle() As String = New String() {"Nothing", "Nothing"}
            Dim myear() As String = New String() {"Nothing", "Nothing"}

            Dim dstring As String = "Nothing" & Chr(34) & "Nothing"
            Dim dar As String() = dstring.Split(Chr(34))

            Dim file As System.IO.File
            Dim write As System.IO.StreamWriter
            write = file.CreateText(savedir)
            write.WriteLine("{\rtf1")
            WriteConsolus("Starting read from " & searchdir)

            ScrapeProgress.Maximum = fiArr.Count
            ScrapeProgress.Value = 0

            For Each fri In fiArr
                Try
                    Dim fn As String = fri.Name
                    Dim fd As String = fri.Directory.Name

                    Dim webclient As New WebClient
                    WriteConsolus("")
                    WriteConsolus("--------------------------------------------------------------------------------")
                    WriteConsolus("~> " & fn & " <~")
                    fn = fn.Replace(fri.Extension.ToString, "")
                    fn = RemoveUnneededData(fn)

                    If fn.Contains("(") And fn.Contains(")") Then
                        mtitle = fn.Split("(")
                        myear = fn.Split("(")
                        myear = myear(1).Split(")")
                        mtitle(0).Replace(myear(0), "")
                        WriteConsolus(" + Accessing " & "http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                        dstring = webclient.DownloadString("http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                        dar = dstring.Split(Chr(34))
                        classif = dar(11)
                        time = dar(19)
                        genre = dar(23)
                        plot = dar(39)
                        rating = dar(47)
                        successful = successful + 1
                    Else
                        If fn.Contains("[") And fn.Contains("]") Then
                            mtitle = fn.Split("[")
                            myear = fn.Split("[")
                            myear = myear(1).Split("]")
                            mtitle(0).Replace(myear(0), "")
                            WriteConsolus(" + Accessing " & "http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                            dstring = webclient.DownloadString("http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                            dar = dstring.Split(Chr(34))
                            classif = dar(11)
                            time = dar(19)
                            genre = dar(23)
                            plot = dar(39)
                            rating = dar(47)
                            successful = successful + 1
                        Else
                            If System.Text.RegularExpressions.Regex.IsMatch(fn, "([0-9][0-9][0-9][0-9])") Then
                                Dim mtemp As String() = fn.Split(" ")
                                myear(0) = mtemp.Last
                                mtitle(0) = Nothing
                                For Each z As String In mtemp
                                    If z IsNot mtemp.Last Then
                                        mtitle(0) = mtitle(0) & z & " "
                                    Else
                                        Exit For
                                    End If
                                Next
                                WriteConsolus(" + Accessing " & "http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                                dstring = webclient.DownloadString("http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                                dar = dstring.Split(Chr(34))
                                classif = dar(11)
                                time = dar(19)
                                genre = dar(23)
                                plot = dar(39)
                                rating = dar(47)
                                successful = successful + 1
                            Else
                                If IsNumeric(fd) OrElse fd.Length = 9 Then
                                    mtitle = fn.Split(".")
                                    Dim tdf As Integer
                                    Dim tdf2 As Integer
                                    If fd.Length <> 4 Then
                                        tdf = CInt(fd.Substring(0, 4))
                                        tdf2 = CInt(fd.Substring(5, 4))
                                        WriteConsolus("Testing multiple year ranges...")
                                        For c As Integer = tdf To tdf2
                                            WriteConsolus(" + Testing " & c.ToString)
                                            myear(0) = c
                                            mtitle(0).Replace(myear(0), "")
                                            WriteConsolus("   - Accessing " & "http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                                            dstring = webclient.DownloadString("http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                                            dar = dstring.Split(Chr(34))
                                            classif = dar(11)
                                            time = dar(19)
                                            genre = dar(23)
                                            plot = dar(39)
                                            rating = dar(47)
                                        Next
                                    Else
                                        myear(0) = fd
                                        mtitle(0).Replace(myear(0), "")
                                        WriteConsolus(" + Accessing " & "http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                                        dstring = webclient.DownloadString("http://www.omdbapi.com/?t=" & mtitle(0) & "&y=" & myear(0))
                                        dar = dstring.Split(Chr(34))
                                        classif = dar(11)
                                        time = dar(19)
                                        genre = dar(23)
                                        plot = dar(39)
                                        rating = dar(47)
                                    End If
                                    successful = successful + 1
                                Else
                                    mtitle = fn.Split(".")
                                    WriteConsolus(" + Accessing " & "http://www.omdbapi.com/?t=" & mtitle(0))
                                    dstring = webclient.DownloadString("http://www.omdbapi.com/?t=" & mtitle(0))
                                    dar = dstring.Split(Chr(34))
                                    classif = dar(11)
                                    time = dar(19)
                                    genre = dar(23)
                                    plot = dar(39)
                                    rating = dar(47)
                                    myear(0) = dar(15)
                                    successful = successful + 1
                                End If
                            End If
                        End If
                    End If

                    write.WriteLine("Title: " & dar(3) & " Year: " & myear(0) & " Classification: " & classif)
                    write.WriteLine("\par")
                    write.WriteLine("Time: " & time & " Genre: " & genre & " IMDb Rating: " & rating & vbCrLf)
                    write.WriteLine("\par")
                    write.WriteLine("Plot Summary: " & plot & vbCrLf & vbCrLf)
                    write.WriteLine("\par")
                    write.WriteLine("\par")
                    WriteConsolus(" + Retrieved Data!")
                Catch ex As Exception
                    Try
                        WriteConsolus(" + Movie not found in year. Attempting to search for just movie title.")
                        Dim fn As String = fri.Name
                        Dim fd As String = fri.Directory.Name
                        Dim webclient As New WebClient
                        myear(0) = Nothing
                        fn = fn.Replace(fri.Extension.ToString, "")
                        fn = RemoveUnneededData(fn)
                        If fn.Contains("(") And fn.Contains(")") Then
                            mtitle = fn.Split("(")
                        Else
                            If fn.Contains("[") And fn.Contains("]") Then
                                mtitle = fn.Split("[")
                            Else
                                If System.Text.RegularExpressions.Regex.IsMatch(fn, "([0-9][0-9][0-9][0-9])") Then
                                    Dim mtemp As String() = fn.Split(" ")
                                    myear(0) = mtemp.Last
                                    mtitle(0) = Nothing
                                    For Each z As String In mtemp
                                        If z IsNot mtemp.Last Then
                                            mtitle(0) = mtitle(0) & z & " "
                                        Else
                                            Exit For
                                        End If
                                    Next
                                Else
                                    If IsNumeric(fd) Then
                                        mtitle = fn.Split(".")
                                    End If
                                End If
                            End If
                        End If
                        WriteConsolus(" + Accessing " & "http://www.omdbapi.com/?t=" & mtitle(0))
                        dstring = webclient.DownloadString("http://www.omdbapi.com/?t=" & mtitle(0))
                        dar = dstring.Split(Chr(34))
                        classif = dar(11)
                        time = dar(19)
                        genre = dar(23)
                        plot = dar(39)
                        rating = dar(47)
                        myear(0) = dar(15)
                        WriteConsolus(" + Title search successful!")
                        successful = successful + 1

                        write.WriteLine("Title: " & dar(3) & " Year: " & myear(0) & " Classification: " & classif)
                        write.WriteLine("\par")
                        write.WriteLine("Time: " & time & " Genre: " & genre & " IMDb Rating: " & rating & vbCrLf)
                        write.WriteLine("\par")
                        write.WriteLine("Plot Summary: " & plot & vbCrLf & vbCrLf)
                        write.WriteLine("\par")
                        write.WriteLine("SCRAPED USING AN AMBIGUOUS METHOD! MAY REQUIRE DOUBLE CHECKING!")
                        write.WriteLine("\par")
                        write.WriteLine("\par")
                        WriteConsolus(" + Retrieved Data!")
                    Catch ex2 As Exception
                        If dar(3) = False And dar(7) = "Movie not found!" Then
                            WriteConsolus(" + Movie was not found, please do a manual lookup by:")
                            WriteConsolus("   - Doing a manual search on IMDb.com")
                            WriteConsolus("   - Or searching on Google.com")
                            moviedetails = moviedetails & "Failed to read: " & fri.Name & " Location: " & fri.FullName & " Reason: Movie not found! Please do a manual lookup!" & "\par" & ex.Message & "\par" & " " & "\par" & " "
                        Else
                            WriteConsolus(ex.Message)
                            moviedetails = moviedetails & "Failed to read: " & fri.Name & " Location: " & fri.FullName & " Reason: " & ex.Message & "\par" & "Variables at time of failure: " & mtitle(0) & "[Title]" & myear(0) & "[Year]" & "\par" & " " & "\par" & " "
                        End If
                        'WriteConsolus("Movie was not found, please do a manual lookup! (Movie might have different name.)")
                        'moviedetails = moviedetails & "Failed to read: " & fri.Name & " Location: " & fri.FullName & " Reason: Movie not found! Please do a manual lookup!" & "\par" & ex.Message & "\par" & " " & "\par" & " "
                    End Try

                End Try
                WriteConsolus("--------------------------------------------------------------------------------")
                total = total + 1
                ScrapeProgress.Value += 1
                PercentComplete.Text = Int(ScrapeProgress.Value / ScrapeProgress.Maximum * 100) & "%"
                NumberOfSuccessful.Text = "Number of successful scrapes: " & successful.ToString & " of " & total.ToString
            Next fri
            write.WriteLine("}")
            write.Close()
            WriteConsolus("")
            WriteConsolus("Finished! RTF File created at " & savedir)
            If moviedetails IsNot Nothing Then
                write = file.CreateText(errorlist)
                write.WriteLine("{\rtf1")
                moviedetails.Replace("\", "/")
                write.Write(moviedetails)
                write.Write("Please change the filename manually and rerun!")
                write.WriteLine("}")
                write.Close()
                WriteConsolus("There were errors reading some files. Please check " & errorlist & " for details.")
            Else
                WriteConsolus("No errors reading filenames.")
            End If
            WriteConsolus("")
            WriteConsolus(">>>>> DISCLAIMER <<<<<")
            WriteConsolus("")
            WriteConsolus("Please note that the results for these movies are NOT from IMDb!")
            WriteConsolus("They are provided by OMDBAPI.com.")
            WriteConsolus("Any likeness of results to persons deceased or living is coincidental!")
            WriteConsolus("")
            WriteConsolus("Distributed as open source by Asryael - http://stackoverflow.com/users/1808539/asryael.")
        Else
            WriteConsolus("No destination and source folders selected!!!")
        End If

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles SelectDestination.Click
        Dim fb As New SaveFileDialog
        fb.Filter = "RTF Files(*.rtf)|*.rtf|All files (*.*)|*.*"
        fb.FilterIndex = 1
        Dim dlgResult As DialogResult = fb.ShowDialog()

        If dlgResult = Windows.Forms.DialogResult.OK Then
            savedir = fb.FileName
            errorlist = fb.FileName.Remove(fb.FileName.LastIndexOf(".")) & " - Failed Reads.rtf"
        End If
    End Sub

    Private Sub WriteConsolus(text As String)
        Consolus.AppendText(text & vbCrLf)
        Consolus.SelectionStart = Consolus.Text.Length
        Consolus.ScrollToCaret()
    End Sub

    Public Function RemoveUnneededData(text As String) As String
        text = text.Replace(".", " ")
        text = text.Replace("720p", "")
        text = text.Replace("1080p", "")
        text = text.Replace(" CD1", "")
        text = text.Replace(" CD2", "")
        text = text.Replace("-CD1", "")
        text = text.Replace("-CD2", "")
        text = text.Replace("CD1", "")
        text = text.Replace("CD2", "")
        text = text.Replace("-cd1", "")
        text = text.Replace("-cd2", "")
        text = text.Replace(" cd1", "")
        text = text.Replace(" cd2", "")
        text = text.Replace("cd1", "")
        text = text.Replace("cd2", "")
        text = text.Replace(" dvd1", "")
        text = text.Replace(" dvd2", "")
        text = text.Replace("-dvd1", "")
        text = text.Replace("-dvd2", "")
        text = text.Replace("dvd1", "")
        text = text.Replace("dvd2", "")
        Return text
    End Function
End Class