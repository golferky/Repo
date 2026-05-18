Imports MailKit.Net.Smtp
Imports MailKit.Security
Imports MimeKit

Public Class frmLeagueMessenger

    Private ohelper As Helper
    Private multipart As New Multipart("mixed")
    Private isTextMode As Boolean = False
    Private dtPlayers As DataTable

    ' Controls
    Private lstAvailable As ListBox
    Private lstSelected As ListBox
    Private tbCC As TextBox
    Private tbBCC As TextBox
    Private tbSubject As TextBox
    Private tbMessage As RichTextBox
    Private tbAttach As TextBox
    Private btnSend As Button
    Private chkEmailMode As RadioButton
    Private chkTextMode As RadioButton
    Private pnlMain As Panel
    Private chkIncludeSubs As CheckBox

    ' ── LOAD ─────────────────────────────────────────────────────────

    Private Sub frmLeagueMessenger_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ohelper = ctx.oHelper
        Me.Text = "League Messenger"
        Me.Size = New Size(940, 820)
        Me.MinimumSize = New Size(900, 750)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.FromArgb(245, 247, 250)
        Me.Font = New Font("Segoe UI", 9)
        InitializeControls()
        LoadAvailablePlayers()
    End Sub

    ' ── BUILD UI ─────────────────────────────────────────────────────

    Private Sub InitializeControls()

        ' ── HEADER ───────────────────────────────────────────────────
        Dim pnlHeader As New Panel With {
        .Dock = DockStyle.Top,
        .Height = 54,
        .BackColor = Color.FromArgb(34, 85, 34)
    }
        Me.Controls.Add(pnlHeader)

        pnlHeader.Controls.Add(New Label With {
        .Text = $"⛳  {ctx.sLeagueName}  —  League Messenger",
        .Font = New Font("Segoe UI", 13, FontStyle.Bold),
        .ForeColor = Color.White,
        .AutoSize = True,
        .Location = New Point(14, 14)
    })

        chkEmailMode = New RadioButton With {
        .Text = "📧 Email",
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .ForeColor = Color.White,
        .Checked = True,
        .AutoSize = True,
        .Location = New Point(680, 18)
    }
        AddHandler chkEmailMode.CheckedChanged, AddressOf ModeChanged
        pnlHeader.Controls.Add(chkEmailMode)

        chkTextMode = New RadioButton With {
        .Text = "📱 Text",
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .ForeColor = Color.White,
        .AutoSize = True,
        .Location = New Point(770, 18)
    }
        AddHandler chkTextMode.CheckedChanged, AddressOf ModeChanged
        pnlHeader.Controls.Add(chkTextMode)

        ' ── MAIN PANEL ───────────────────────────────────────────────
        pnlMain = New Panel With {
        .Location = New Point(0, 54),
        .Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height - 54),
        .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom,
        .AutoScroll = False
    }
        Me.Controls.Add(pnlMain)

        ' ── AVAILABLE PLAYERS ─────────────────────────────────────────
        Dim grpAvail As New GroupBox With {
        .Text = "Available Players",
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .ForeColor = Color.FromArgb(34, 85, 34),
        .Location = New Point(10, 10),
        .Size = New Size(240, 322),
        .Anchor = AnchorStyles.Top Or AnchorStyles.Left
    }
        pnlMain.Controls.Add(grpAvail)

        lstAvailable = New ListBox With {
        .Location = New Point(6, 22),
        .Size = New Size(226, 228),
        .SelectionMode = SelectionMode.MultiExtended,
        .Font = New Font("Segoe UI", 8.5),
        .BorderStyle = BorderStyle.FixedSingle
    }
        AddHandler lstAvailable.MouseDoubleClick, Sub(s, ev) MoveToSelected()
        grpAvail.Controls.Add(lstAvailable)

        ' ── INCLUDE SUBS CHECKBOX ────────────────────────────────────
        chkIncludeSubs = New CheckBox With {
        .Name = "chkIncludeSubs",
        .Text = "Include Subs",
        .Font = New Font("Segoe UI", 8.5),
        .AutoSize = True,
        .Location = New Point(6, 256)
    }
        AddHandler chkIncludeSubs.CheckedChanged, Sub(s, e) LoadAvailablePlayers()
        grpAvail.Controls.Add(chkIncludeSubs)

        Dim btnSelAll As New Button With {
        .Text = "✔ All",
        .Size = New Size(72, 28),
        .Location = New Point(6, 282),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(210, 235, 210),
        .Font = New Font("Segoe UI", 8, FontStyle.Bold)
    }
        AddHandler btnSelAll.Click, Sub(s, ev)
                                        For ix = 0 To lstAvailable.Items.Count - 1
                                            lstAvailable.SetSelected(ix, True)
                                        Next
                                    End Sub
        grpAvail.Controls.Add(btnSelAll)

        Dim btnDeselAll As New Button With {
        .Text = "✖ None",
        .Size = New Size(72, 28),
        .Location = New Point(82, 282),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(235, 210, 210),
        .Font = New Font("Segoe UI", 8, FontStyle.Bold)
    }
        AddHandler btnDeselAll.Click, Sub(s, ev) lstAvailable.ClearSelected()
        grpAvail.Controls.Add(btnDeselAll)

        Dim btnRefresh As New Button With {
        .Text = "↺",
        .Size = New Size(72, 28),
        .Location = New Point(158, 282),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(220, 230, 245),
        .Font = New Font("Segoe UI", 9, FontStyle.Bold)
    }
        AddHandler btnRefresh.Click, Sub(s, ev) LoadAvailablePlayers()
        grpAvail.Controls.Add(btnRefresh)

        ' ── ARROW BUTTONS ─────────────────────────────────────────────
        Dim pnlArrows As New Panel With {
        .Location = New Point(254, 85),
        .Size = New Size(56, 175),
        .Anchor = AnchorStyles.Top Or AnchorStyles.Left
    }
        pnlMain.Controls.Add(pnlArrows)

        Dim arrowDefs As (txt As String, act As Action)() = {
        (">", New Action(AddressOf MoveToSelected)),
        (">>", New Action(AddressOf MoveAllToSelected)),
        ("<", New Action(AddressOf MoveToAvailable)),
        ("<<", New Action(AddressOf MoveAllToAvailable))
    }
        Dim ay As Integer = 0
        For Each def In arrowDefs
            Dim localAct = def.act
            Dim btnArr As New Button With {
            .Text = def.txt,
            .Size = New Size(50, 34),
            .Location = New Point(3, ay),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(34, 85, 34),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 9, FontStyle.Bold)
        }
            AddHandler btnArr.Click, Sub(s, ev) localAct()
            pnlArrows.Controls.Add(btnArr)
            ay += 40
        Next

        ' ── SELECTED RECIPIENTS ───────────────────────────────────────
        Dim grpSel As New GroupBox With {
        .Text = "Selected Recipients",
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .ForeColor = Color.FromArgb(34, 85, 34),
        .Location = New Point(314, 10),
        .Size = New Size(240, 322),
        .Anchor = AnchorStyles.Top Or AnchorStyles.Left
    }
        pnlMain.Controls.Add(grpSel)

        lstSelected = New ListBox With {
        .Location = New Point(6, 22),
        .Size = New Size(226, 290),
        .SelectionMode = SelectionMode.MultiExtended,
        .Font = New Font("Segoe UI", 8.5),
        .BorderStyle = BorderStyle.FixedSingle
    }
        AddHandler lstSelected.MouseDoubleClick, Sub(s, ev) MoveToAvailable()
        grpSel.Controls.Add(lstSelected)

        ' ── TIPS PANEL ───────────────────────────────────────────────
        Dim pnlTips As New Panel With {
        .Location = New Point(562, 10),
        .Size = New Size(346, 322),
        .BackColor = Color.FromArgb(240, 248, 240),
        .BorderStyle = BorderStyle.FixedSingle,
        .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
    }
        pnlMain.Controls.Add(pnlTips)

        pnlTips.Controls.Add(New Label With {
        .Text = "💡 Quick Tips",
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .ForeColor = Color.FromArgb(34, 85, 34),
        .Location = New Point(10, 8),
        .AutoSize = True
    })
        pnlTips.Controls.Add(New Label With {
        .Text = "• Double-click a player to move them" & vbCrLf &
                "• >> moves all players at once" & vbCrLf &
                "• Select text then click B / I / U to wrap tags" & vbCrLf &
                "• Click 🎨 Color to wrap selected text in color" & vbCrLf &
                "• Preview shows the finished rendered email" & vbCrLf &
                "• CC / BCC: separate addresses with semicolons" & vbCrLf &
                "• Attach supports PDF, images, spreadsheets" & vbCrLf &
                "• Switch to 📱 Text to send SMS via carrier gateway" & vbCrLf &
                "• Check Include Subs to add sub players to list",
        .Font = New Font("Segoe UI", 8.5),
        .ForeColor = Color.FromArgb(55, 55, 55),
        .Location = New Point(10, 30),
        .Size = New Size(326, 282)
    })

        ' ── TO / CC / BCC / SUBJECT ──────────────────────────────────
        Dim y As Integer = 341
        For Each fld As (lbl As String, name As String, ph As String) In {
        ("CC:", "CC", "Carbon copy — separate multiple with ;"),
        ("BCC:", "BCC", "Blind carbon copy — separate multiple with ;"),
        ("Subject:", "Subject", "Email subject line")
    }
            pnlMain.Controls.Add(New Label With {
            .Text = fld.lbl,
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(34, 85, 34),
            .Location = New Point(10, y + 4),
            .Width = 58
        })
            Dim tb As New TextBox With {
            .Name = $"tb{fld.name}",
            .Location = New Point(72, y),
            .Width = 836,
            .Font = New Font("Segoe UI", 9),
            .BorderStyle = BorderStyle.FixedSingle,
            .PlaceholderText = fld.ph,
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        }
            pnlMain.Controls.Add(tb)
            Select Case fld.name
                Case "CC" : tbCC = tb
                Case "BCC" : tbBCC = tb
                Case "Subject" : tbSubject = tb
            End Select
            y += 30
        Next

        ' ── HTML TOOLBAR ─────────────────────────────────────────────
        Dim pnlToolbar As New Panel With {
        .Location = New Point(10, y),
        .Size = New Size(898, 34),
        .BackColor = Color.FromArgb(228, 240, 228),
        .BorderStyle = BorderStyle.FixedSingle,
        .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
    }
        pnlMain.Controls.Add(pnlToolbar)

        Dim tx As Integer = 4
        Dim fmtDefs As (lbl As String, o As String, c As String, bold As Boolean, italic As Boolean)() = {
        ("B", "<strong>", "</strong>", True, False),
        ("I", "<em>", "</em>", False, True),
        ("U", "<u>", "</u>", False, False),
        ("H2", "<h2>", "</h2>", True, False),
        ("H3", "<h3>", "</h3>", True, False),
        ("• List", "<li>", "</li>", False, False),
        ("── HR", "<hr/>", "", False, False)
    }
        For Each fmt In fmtDefs
            Dim lo = fmt.o, lc = fmt.c
            Dim w = If(fmt.lbl.Length <= 2, 32, 62)
            Dim btnFmt As New Button With {
            .Text = fmt.lbl,
            .Size = New Size(w, 26),
            .Location = New Point(tx, 3),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White
        }
            If fmt.bold Then btnFmt.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            If fmt.italic Then btnFmt.Font = New Font("Segoe UI", 9, FontStyle.Italic)
            AddHandler btnFmt.Click, Sub(s, ev)
                                         Dim sel = tbMessage.SelectedText
                                         tbMessage.SelectedText = If(lc = "", lo, lo & sel & lc)
                                         tbMessage.Focus()
                                     End Sub
            pnlToolbar.Controls.Add(btnFmt)
            tx += w + 3
        Next

        Dim btnGreen As New Button With {
        .Text = "🟢 Green",
        .Size = New Size(76, 26),
        .Location = New Point(tx, 3),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.White
    }
        AddHandler btnGreen.Click, Sub(s, ev)
                                       tbMessage.SelectedText = $"<span style='color:#225522'>{tbMessage.SelectedText}</span>"
                                       tbMessage.Focus()
                                   End Sub
        pnlToolbar.Controls.Add(btnGreen)
        tx += 80

        Dim btnClr As New Button With {
        .Text = "🎨 Color",
        .Size = New Size(76, 26),
        .Location = New Point(tx, 3),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.White
    }
        AddHandler btnClr.Click, Sub(s, ev)
                                     Using cd As New ColorDialog()
                                         If cd.ShowDialog() = DialogResult.OK Then
                                             Dim hex = $"#{cd.Color.R:X2}{cd.Color.G:X2}{cd.Color.B:X2}"
                                             tbMessage.SelectedText = $"<span style='color:{hex}'>{tbMessage.SelectedText}</span>"
                                             tbMessage.Focus()
                                         End If
                                     End Using
                                 End Sub
        pnlToolbar.Controls.Add(btnClr)
        y += 38

        ' ── MESSAGE HINT ─────────────────────────────────────────────
        pnlMain.Controls.Add(New Label With {
        .Text = "Message Body  (use toolbar to insert HTML formatting tags)",
        .Font = New Font("Segoe UI", 8, FontStyle.Italic),
        .ForeColor = Color.Gray,
        .AutoSize = True,
        .Location = New Point(10, y)
    })
        y += 16

        ' ── MESSAGE BODY ─────────────────────────────────────────────
        tbMessage = New RichTextBox With {
        .Name = "tbMessage",
        .Location = New Point(10, y),
        .Size = New Size(898, 175),
        .Font = New Font("Consolas", 9),
        .BorderStyle = BorderStyle.FixedSingle,
        .AcceptsTab = True,
        .ScrollBars = RichTextBoxScrollBars.Vertical,
        .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
    }
        pnlMain.Controls.Add(tbMessage)
        y += 183

        ' ── ATTACHMENTS ──────────────────────────────────────────────
        tbAttach = New TextBox With {
        .Location = New Point(10, y),
        .Width = 700,
        .ReadOnly = True,
        .PlaceholderText = "No attachments — click 📎 to add files...",
        .Font = New Font("Segoe UI", 8.5),
        .BorderStyle = BorderStyle.FixedSingle,
        .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
    }
        pnlMain.Controls.Add(tbAttach)

        Dim btnClearAtt As New Button With {
        .Text = "✖",
        .Size = New Size(26, 26),
        .Location = New Point(714, y),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(235, 210, 210),
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
    }
        AddHandler btnClearAtt.Click, Sub(s, ev)
                                          multipart = New Multipart("mixed")
                                          tbAttach.Text = ""
                                      End Sub
        pnlMain.Controls.Add(btnClearAtt)

        Dim btnAtt As New Button With {
        .Text = "📎 Attach File",
        .Size = New Size(166, 26),
        .Location = New Point(744, y),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(215, 225, 255),
        .Font = New Font("Segoe UI", 8.5, FontStyle.Bold),
        .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
    }
        AddHandler btnAtt.Click, AddressOf BtnAttach_Click
        pnlMain.Controls.Add(btnAtt)
        y += 34

        ' ── BOTTOM BUTTONS ───────────────────────────────────────────
        Dim btnPreview As New Button With {
        .Text = "👁  Preview",
        .Size = New Size(120, 38),
        .Location = New Point(10, y),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(195, 215, 255),
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
    }
        AddHandler btnPreview.Click, AddressOf BtnPreview_Click
        pnlMain.Controls.Add(btnPreview)

        btnSend = New Button With {
        .Text = "✉  Send Email",
        .Size = New Size(140, 38),
        .Location = New Point(138, y),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(34, 120, 34),
        .ForeColor = Color.White,
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
    }
        AddHandler btnSend.Click, AddressOf BtnSend_Click
        pnlMain.Controls.Add(btnSend)

        Dim btnExit As New Button With {
        .Text = "✖  Close",
        .Size = New Size(100, 38),
        .Location = New Point(286, y),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(190, 50, 50),
        .ForeColor = Color.White,
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
    }
        AddHandler btnExit.Click, Sub(s, ev) Me.Close()
        pnlMain.Controls.Add(btnExit)

    End Sub

    ' ── PLAYER PANEL LOGIC ───────────────────────────────────────────

    Private Sub LoadAvailablePlayers()
        lstAvailable.Items.Clear()
        lstSelected.Items.Clear()

        Dim sql As String
        If isTextMode Then
            sql = $"SELECT P.Player, P.Phone, P.CellCarrier
            FROM Players P
            {If(Not chkIncludeSubs.Checked, $"JOIN Teams T ON T.Player = P.Player AND T.Year = {ctx.SeasonYear}", "")}
            WHERE P.TextStats = 'Y'
            AND P.Phone <> '' AND P.CellCarrier <> '' AND P.CellCarrier <> 'None'
            ORDER BY P.Player"
        Else
            sql = $"SELECT P.Player, P.Email, P.Phone, P.CellCarrier
            FROM Players P
            {If(Not chkIncludeSubs.Checked, $"JOIN Teams T ON T.Player = P.Player AND T.Year = {ctx.SeasonYear}", "")}
            WHERE P.EmailStats = 'Y'
            AND (P.Email <> '' OR P.Phone <> '')
            ORDER BY P.Player"
        End If

        dtPlayers = ohelper.sqlitedaFromSql(ctx.Conn, "", sql)
        For Each row As DataRow In dtPlayers.Rows
            lstAvailable.Items.Add(row("Player").ToString())
        Next
        btnSend.Text = If(isTextMode, "📱  Send Text", "✉  Send Email")
    End Sub

    Private Sub MoveToSelected()
        Dim toMove As New List(Of Object)
        For Each item As Object In lstAvailable.SelectedItems
            toMove.Add(item)
        Next
        For Each item As Object In toMove
            lstAvailable.Items.Remove(item)
            If Not lstSelected.Items.Contains(item) Then lstSelected.Items.Add(item)
        Next
    End Sub

    Private Sub MoveAllToSelected()
        For Each item As Object In lstAvailable.Items
            If Not lstSelected.Items.Contains(item) Then lstSelected.Items.Add(item)
        Next
        lstAvailable.Items.Clear()
    End Sub

    Private Sub MoveToAvailable()
        Dim toMove As New List(Of Object)
        For Each item As Object In lstSelected.SelectedItems
            toMove.Add(item)
        Next
        For Each item As Object In toMove
            lstSelected.Items.Remove(item)
            If Not lstAvailable.Items.Contains(item) Then lstAvailable.Items.Add(item)
        Next
    End Sub

    Private Sub MoveAllToAvailable()
        For Each item As Object In lstSelected.Items
            If Not lstAvailable.Items.Contains(item) Then lstAvailable.Items.Add(item)
        Next
        lstSelected.Items.Clear()
    End Sub

    ' ── HTML BUILDER ─────────────────────────────────────────────────

    Private Function BuildHtmlEmail() As String
        Return $"<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'/>
<style>
  body {{ margin:0; padding:0; background:#f0f0f0; font-family:'Segoe UI',Arial,sans-serif; }}
  .wrapper {{ max-width:640px; margin:30px auto; background:#ffffff;
              border-radius:10px; overflow:hidden;
              box-shadow:0 4px 18px rgba(0,0,0,0.15); }}
  .hdr {{ background:linear-gradient(135deg,#1a4d1a,#2e7d2e);
          padding:26px 32px; color:#ffffff; }}
  .hdr h1 {{ margin:0 0 4px 0; font-size:24px; letter-spacing:0.5px; }}
  .hdr p  {{ margin:0; font-size:12px; opacity:0.7; }}
  .body {{ padding:30px 36px; color:#333333; font-size:15px; line-height:1.8; }}
  .body h2 {{ color:#1a5c1a; border-bottom:2px solid #d4edda; padding-bottom:6px; }}
  .body h3 {{ color:#2e7d2e; }}
  .body hr  {{ border:none; border-top:1px solid #e0e0e0; margin:22px 0; }}
  .body li  {{ margin-bottom:6px; }}
  .footer {{ background:#f7f7f7; border-top:1px solid #e0e0e0;
             padding:14px 32px; font-size:11px; color:#999999; text-align:center; }}
</style>
</head>
<body>
<div class='wrapper'>
  <div class='hdr'>
    <h1>⛳ {ctx.sLeagueName}</h1>
    <p>{DateTime.Now:dddd, MMMM d, yyyy}</p>
  </div>
  <div class='body'>
    {tbMessage.Text.Replace(vbCrLf, "<br/>").Replace(vbLf, "<br/>")}
  </div>
  <div class='footer'>
    Sent by {ctx.sLeagueName} League Management &nbsp;|&nbsp; {DateTime.Now.Year}
  </div>
</div>
</body>
</html>"
    End Function

    ' ── PREVIEW ──────────────────────────────────────────────────────

    Private Sub BtnPreview_Click(sender As Object, e As EventArgs)
        Dim frmPrev As New Form With {
            .Text = $"Preview — {tbSubject.Text}",
            .Size = New Size(700, 650),
            .StartPosition = FormStartPosition.CenterParent,
            .BackColor = Color.White,
            .MinimizeBox = False,
            .MaximizeBox = False
        }
        Dim pnlPrevTop As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 36,
            .BackColor = Color.FromArgb(240, 248, 240),
            .Padding = New Padding(6, 4, 6, 4)
        }
        pnlPrevTop.Controls.Add(New Label With {
            .Text = $"To: {lstSelected.Items.Count} recipient(s)   |   Subject: {tbSubject.Text}",
            .Font = New Font("Segoe UI", 8.5, FontStyle.Italic),
            .ForeColor = Color.FromArgb(34, 85, 34),
            .AutoSize = True,
            .Location = New Point(8, 8)
        })
        frmPrev.Controls.Add(pnlPrevTop)

        Dim browser As New WebBrowser With {
            .Dock = DockStyle.Fill,
            .ScrollBarsEnabled = True
        }
        frmPrev.Controls.Add(browser)
        frmPrev.Show()
        browser.DocumentText = BuildHtmlEmail()
    End Sub

    ' ── SEND ─────────────────────────────────────────────────────────

    Private Sub BtnSend_Click(sender As Object, e As EventArgs)
        If lstSelected.Items.Count = 0 Then
            MessageBox.Show("Please move at least one player to the Selected Recipients panel.",
                            "No Recipients", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        If String.IsNullOrWhiteSpace(tbSubject.Text) Then
            MessageBox.Show("Please enter a subject line.",
                            "Missing Subject", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        If String.IsNullOrWhiteSpace(tbMessage.Text) Then
            If MessageBox.Show("Message body is empty. Send anyway?",
                               "Empty Message", MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub
        End If

        Dim mode = If(isTextMode, "text message", "email")
        If MessageBox.Show($"Send {mode} to {lstSelected.Items.Count} recipient(s)?",
                           "Confirm Send", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

        Dim sentCount As Integer = 0
        Dim failList As New List(Of String)

        Try
            Dim message As New MimeMessage()
            message.From.Add(New MailboxAddress(ctx.sLeagueName,
                                                ctx.rLeagueParmrow("email").ToString()))
            message.Subject = tbSubject.Text

            ' CC
            If Not String.IsNullOrWhiteSpace(tbCC.Text) Then
                For Each addr In tbCC.Text.Split(";"c)
                    Dim clean = addr.Trim()
                    If clean <> "" Then message.Cc.Add(New MailboxAddress("", clean))
                Next
            End If

            ' BCC
            If Not String.IsNullOrWhiteSpace(tbBCC.Text) Then
                For Each addr In tbBCC.Text.Split(";"c)
                    Dim clean = addr.Trim()
                    If clean <> "" Then message.Bcc.Add(New MailboxAddress("", clean))
                Next
            End If

            ' Recipients
            For Each playerName As Object In lstSelected.Items
                Dim pName = playerName.ToString()
                Dim pRows = dtPlayers.Select($"Player = '{pName.Replace("'", "''")}'")
                If pRows.Length = 0 Then
                    failList.Add($"{pName} (not found)")
                    Continue For
                End If
                Dim pRow = pRows(0)
                If isTextMode Then
                    Dim textAddr = ohelper.TextNumber(pName)
                    If Not String.IsNullOrWhiteSpace(textAddr) Then
                        message.To.Add(New MailboxAddress(pName, textAddr))
                        sentCount += 1
                    Else
                        failList.Add($"{pName} (no text address)")
                    End If
                Else
                    Dim emailAddr = pRow("Email").ToString()
                    If Not String.IsNullOrWhiteSpace(emailAddr) Then
                        message.To.Add(New MailboxAddress(pName, emailAddr))
                        sentCount += 1
                    Else
                        failList.Add($"{pName} (no email)")
                    End If
                End If
            Next

            If message.To.Count = 0 Then
                MessageBox.Show("No valid addresses found for any selected player.",
                                "Nothing to Send", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Body
            Dim mp As New Multipart("mixed")
            If isTextMode Then
                mp.Add(New TextPart("plain") With {.Text = tbMessage.Text})
            Else
                mp.Add(New TextPart("html") With {.Text = BuildHtmlEmail()})
            End If
            For Each att As MimePart In multipart.OfType(Of MimePart)()
                mp.Add(att)
            Next
            message.Body = mp

            ' Send
            Using client As New SmtpClient()
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls)
                client.Authenticate(ctx.rLeagueParmrow("email").ToString(),
                                    ctx.rLeagueParmrow("emailpassword").ToString())
                client.Send(message)
                client.Disconnect(True)
            End Using

            Dim summary = $"✅ {mode} sent to {sentCount} recipient(s)."
            If failList.Count > 0 Then
                summary &= $"{vbCrLf}{vbCrLf}⚠ Skipped ({failList.Count}):{vbCrLf}" &
                           String.Join(vbCrLf, failList)
            End If
            MessageBox.Show(summary, "Send Complete", MessageBoxButtons.OK,
                            MessageBoxIcon.Information)

            ' Reset attachments
            multipart = New Multipart("mixed")
            tbAttach.Text = ""

        Catch ex As Exception
            MessageBox.Show("Error sending: " & ex.Message, "Send Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            LOGIT("frmLeagueMessenger BtnSend Error: " & ex.Message)
        End Try
    End Sub

    ' ── ATTACH ───────────────────────────────────────────────────────

    Private Sub BtnAttach_Click(sender As Object, e As EventArgs)
        Using ofd As New OpenFileDialog With {
            .Title = "Select Attachment(s)",
            .Filter = "Documents (*.pdf;*.xlsx;*.docx)|*.pdf;*.xlsx;*.docx" &
                      "|Images (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg" &
                      "|All Files (*.*)|*.*",
            .Multiselect = True
        }
            If ofd.ShowDialog() = DialogResult.OK Then
                For Each file In ofd.FileNames
                    multipart.Add(New MimePart("application", "octet-stream") With {
                        .Content = New MimeContent(System.IO.File.OpenRead(file)),
                        .ContentDisposition = New ContentDisposition(ContentDisposition.Attachment),
                        .ContentTransferEncoding = ContentEncoding.Base64,
                        .FileName = System.IO.Path.GetFileName(file)
                    })
                    tbAttach.Text &= System.IO.Path.GetFileName(file) & ";  "
                Next
            End If
        End Using
    End Sub

    ' ── MODE TOGGLE ──────────────────────────────────────────────────

    Private Sub ModeChanged(sender As Object, e As EventArgs)
        If Not Me.IsHandleCreated Then Exit Sub
        isTextMode = chkTextMode.Checked
        LoadAvailablePlayers()
    End Sub

End Class