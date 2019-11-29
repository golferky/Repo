<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.PercentComplete = New System.Windows.Forms.Label()
        Me.NumberOfMovies = New System.Windows.Forms.Label()
        Me.NumberOfSuccessful = New System.Windows.Forms.Label()
        Me.Consolus = New System.Windows.Forms.RichTextBox()
        Me.SelectSource = New System.Windows.Forms.Button()
        Me.SelectDestination = New System.Windows.Forms.Button()
        Me.StartScraping = New System.Windows.Forms.Button()
        Me.ScrapeProgress = New System.Windows.Forms.ProgressBar()
        Me.SuspendLayout()
        '
        'PercentComplete
        '
        Me.PercentComplete.AutoSize = True
        Me.PercentComplete.Location = New System.Drawing.Point(12, 32)
        Me.PercentComplete.Name = "PercentComplete"
        Me.PercentComplete.Size = New System.Drawing.Size(21, 13)
        Me.PercentComplete.TabIndex = 0
        Me.PercentComplete.Text = "0%"
        '
        'NumberOfMovies
        '
        Me.NumberOfMovies.AutoSize = True
        Me.NumberOfMovies.Location = New System.Drawing.Point(12, 61)
        Me.NumberOfMovies.Name = "NumberOfMovies"
        Me.NumberOfMovies.Size = New System.Drawing.Size(149, 13)
        Me.NumberOfMovies.TabIndex = 1
        Me.NumberOfMovies.Text = "Number Of Movies in directory"
        '
        'NumberOfSuccessful
        '
        Me.NumberOfSuccessful.AutoSize = True
        Me.NumberOfSuccessful.Location = New System.Drawing.Point(12, 98)
        Me.NumberOfSuccessful.Name = "NumberOfSuccessful"
        Me.NumberOfSuccessful.Size = New System.Drawing.Size(155, 13)
        Me.NumberOfSuccessful.TabIndex = 2
        Me.NumberOfSuccessful.Text = "Number of successful scrapes: "
        '
        'Consolus
        '
        Me.Consolus.DetectUrls = False
        Me.Consolus.Font = New System.Drawing.Font("Consolas", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Consolus.Location = New System.Drawing.Point(15, 124)
        Me.Consolus.Name = "Consolus"
        Me.Consolus.Size = New System.Drawing.Size(100, 96)
        Me.Consolus.TabIndex = 3
        Me.Consolus.Text = ""
        '
        'SelectSource
        '
        Me.SelectSource.Location = New System.Drawing.Point(397, 126)
        Me.SelectSource.Name = "SelectSource"
        Me.SelectSource.Size = New System.Drawing.Size(145, 23)
        Me.SelectSource.TabIndex = 4
        Me.SelectSource.Text = "Select Source"
        Me.SelectSource.UseVisualStyleBackColor = True
        '
        'SelectDestination
        '
        Me.SelectDestination.Location = New System.Drawing.Point(397, 185)
        Me.SelectDestination.Name = "SelectDestination"
        Me.SelectDestination.Size = New System.Drawing.Size(145, 23)
        Me.SelectDestination.TabIndex = 5
        Me.SelectDestination.Text = "Select Destination"
        Me.SelectDestination.UseVisualStyleBackColor = True
        '
        'StartScraping
        '
        Me.StartScraping.Location = New System.Drawing.Point(397, 243)
        Me.StartScraping.Name = "StartScraping"
        Me.StartScraping.Size = New System.Drawing.Size(145, 23)
        Me.StartScraping.TabIndex = 6
        Me.StartScraping.Text = "Start Scraping"
        Me.StartScraping.UseVisualStyleBackColor = True
        '
        'ScrapeProgress
        '
        Me.ScrapeProgress.Location = New System.Drawing.Point(15, 358)
        Me.ScrapeProgress.Name = "ScrapeProgress"
        Me.ScrapeProgress.Size = New System.Drawing.Size(267, 23)
        Me.ScrapeProgress.TabIndex = 7
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(741, 404)
        Me.Controls.Add(Me.ScrapeProgress)
        Me.Controls.Add(Me.StartScraping)
        Me.Controls.Add(Me.SelectDestination)
        Me.Controls.Add(Me.SelectSource)
        Me.Controls.Add(Me.Consolus)
        Me.Controls.Add(Me.NumberOfSuccessful)
        Me.Controls.Add(Me.NumberOfMovies)
        Me.Controls.Add(Me.PercentComplete)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PercentComplete As Label
    Friend WithEvents NumberOfMovies As Label
    Friend WithEvents NumberOfSuccessful As Label
    Friend WithEvents Consolus As RichTextBox
    Friend WithEvents SelectSource As Button
    Friend WithEvents SelectDestination As Button
    Friend WithEvents StartScraping As Button
    Friend WithEvents ScrapeProgress As ProgressBar
End Class
