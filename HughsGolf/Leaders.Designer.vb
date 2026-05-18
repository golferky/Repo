<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Leaders
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        dgEagles = New DataGridView()
        Label2 = New Label()
        CTPS = New Label()
        dgCTP = New DataGridView()
        cbDates = New ComboBox()
        Label7 = New Label()
        Label11 = New Label()
        dgDbBogies = New DataGridView()
        Label12 = New Label()
        dgMax = New DataGridView()
        cbRegulars = New CheckBox()
        BindingSource1 = New BindingSource(components)
        CType(dgEagles, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgCTP, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgDbBogies, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgMax, ComponentModel.ISupportInitialize).BeginInit()
        CType(BindingSource1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' dgEagles
        ' 
        dgEagles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgEagles.Location = New Point(2303, 165)
        dgEagles.Margin = New Padding(4, 3, 4, 3)
        dgEagles.Name = "dgEagles"
        dgEagles.Size = New Size(192, 257)
        dgEagles.TabIndex = 2
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(2359, 127)
        Label2.Margin = New Padding(4, 0, 4, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(40, 15)
        Label2.TabIndex = 3
        Label2.Text = "Eagles"
        ' 
        ' CTPS
        ' 
        CTPS.AutoSize = True
        CTPS.Location = New Point(2380, 437)
        CTPS.Margin = New Padding(4, 0, 4, 0)
        CTPS.Name = "CTPS"
        CTPS.Size = New Size(35, 15)
        CTPS.TabIndex = 13
        CTPS.Text = "CTPS"
        ' 
        ' dgCTP
        ' 
        dgCTP.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgCTP.Location = New Point(2305, 480)
        dgCTP.Margin = New Padding(4, 3, 4, 3)
        dgCTP.Name = "dgCTP"
        dgCTP.Size = New Size(190, 653)
        dgCTP.TabIndex = 12
        ' 
        ' cbDates
        ' 
        cbDates.FormattingEnabled = True
        cbDates.Location = New Point(31, 29)
        cbDates.Margin = New Padding(4, 3, 4, 3)
        cbDates.Name = "cbDates"
        cbDates.Size = New Size(111, 23)
        cbDates.TabIndex = 31
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Location = New Point(28, 10)
        Label7.Margin = New Padding(4, 0, 4, 0)
        Label7.Name = "Label7"
        Label7.Size = New Size(64, 15)
        Label7.TabIndex = 30
        Label7.Text = "Stat Period"
        ' 
        ' Label11
        ' 
        Label11.AutoSize = True
        Label11.Location = New Point(2124, 127)
        Label11.Margin = New Padding(4, 0, 4, 0)
        Label11.Name = "Label11"
        Label11.Size = New Size(63, 15)
        Label11.TabIndex = 40
        Label11.Text = "Dbl Bogies"
        ' 
        ' dgDbBogies
        ' 
        dgDbBogies.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgDbBogies.Location = New Point(2068, 165)
        dgDbBogies.Margin = New Padding(4, 3, 4, 3)
        dgDbBogies.Name = "dgDbBogies"
        dgDbBogies.Size = New Size(192, 459)
        dgDbBogies.TabIndex = 39
        ' 
        ' Label12
        ' 
        Label12.AutoSize = True
        Label12.Location = New Point(2124, 640)
        Label12.Margin = New Padding(4, 0, 4, 0)
        Label12.Name = "Label12"
        Label12.Size = New Size(42, 15)
        Label12.TabIndex = 42
        Label12.Text = "Others"
        ' 
        ' dgMax
        ' 
        dgMax.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgMax.Location = New Point(2068, 678)
        dgMax.Margin = New Padding(4, 3, 4, 3)
        dgMax.Name = "dgMax"
        dgMax.Size = New Size(192, 455)
        dgMax.TabIndex = 41
        ' 
        ' cbRegulars
        ' 
        cbRegulars.AutoSize = True
        cbRegulars.Checked = True
        cbRegulars.CheckState = CheckState.Checked
        cbRegulars.Location = New Point(243, 25)
        cbRegulars.Margin = New Padding(4, 3, 4, 3)
        cbRegulars.Name = "cbRegulars"
        cbRegulars.Size = New Size(97, 19)
        cbRegulars.TabIndex = 43
        cbRegulars.Text = "Regulars only"
        cbRegulars.UseVisualStyleBackColor = True
        ' 
        ' Leaders
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1532, 570)
        Controls.Add(cbRegulars)
        Controls.Add(Label12)
        Controls.Add(dgMax)
        Controls.Add(Label11)
        Controls.Add(dgDbBogies)
        Controls.Add(cbDates)
        Controls.Add(Label7)
        Controls.Add(CTPS)
        Controls.Add(dgCTP)
        Controls.Add(Label2)
        Controls.Add(dgEagles)
        Margin = New Padding(4, 3, 4, 3)
        Name = "Leaders"
        Text = "Leaders"
        CType(dgEagles, ComponentModel.ISupportInitialize).EndInit()
        CType(dgCTP, ComponentModel.ISupportInitialize).EndInit()
        CType(dgDbBogies, ComponentModel.ISupportInitialize).EndInit()
        CType(dgMax, ComponentModel.ISupportInitialize).EndInit()
        CType(BindingSource1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents dgEagles As DataGridView
    Friend WithEvents Label2 As Label
    Friend WithEvents CTPS As Label
    Friend WithEvents dgCTP As DataGridView
    Friend WithEvents cbDates As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents dgDbBogies As DataGridView
    Friend WithEvents Label12 As Label
    Friend WithEvents dgMax As DataGridView
    Friend WithEvents cbRegulars As CheckBox
    Friend WithEvents BindingSource1 As BindingSource
End Class
