'Resize form relatively 
'modify size of controls on form 
'change location of controls dynamically as width or height  
'changes on the form 
'class designed by MathLover, free to be used by anybody 
'if you need explanations about anything 
'please feel free to contact me at 
'  mathlover47@hotmail.com 
Public Class Relative
    Private OriginalWidthForm, OriginalHeightForm As UInt32
    Private NewWidthForm, NewHeightForm As UInt32
    Private ControlSizeLocation(,) As Point
    Private SizeLocationContainer(,,) As Point
    Private fontContainer(,) As UInt16
    Private f1 As Form
    Public Sub New(ByRef f12 As Form)
        ' j here reflects the number of containers inside form 
        Static j = 0
        OriginalWidthForm = f12.Width
        OriginalHeightForm = f12.Height
        f1 = f12
        ReDim ControlSizeLocation(f1.Controls.Count, 1)

        ReDim Preserve SizeLocationContainer(120, 1, 120)
        ReDim fontContainer(121, 120)

        For i = 0 To f1.Controls.Count - 1
            ControlSizeLocation(i, 0) = New Point(f1.Controls(i).Size.Width, f1.Controls(i).Size.Height)
            ControlSizeLocation(i, 1) = New Point(f1.Controls(i).Location.X, f1.Controls(i).Location.Y)
            fontContainer(i, 0) = f1.Controls(i).Font.Size

            If IsContainer(f1.Controls(i)) Then
                For a1 = 0 To f1.Controls(i).Controls.Count - 1
                    SizeLocationContainer(a1, 0, j) = New Point(f1.Controls(i).Controls(a1).Size.Width, f1.Controls(i).Controls(a1).Size.Height)
                    SizeLocationContainer(a1, 1, j) = New Point(f1.Controls(i).Controls(a1).Location.X, f1.Controls(i).Controls(a1).Location.Y)
                    fontContainer(a1, j + 1) = f1.Controls(i).Controls(a1).Font.Size
                Next
                j += 1
            End If
        Next



        AddHandler f1.Resize, AddressOf NewFormResizer
    End Sub
    Private Sub NewFormResizer()
        NewWidthForm = f1.Width
        NewHeightForm = f1.Height
        ModifyControls()

    End Sub
    Private Sub ModifyControls()
        Dim ControlsCount As UInt16 = f1.Controls.Count - 1
        Dim n As UInt16 = 0
        For i = 0 To ControlsCount


            f1.Controls(i).Location = ReLocater(ControlSizeLocation(i, 1))
            f1.Controls(i).Size = ReSizer(ControlSizeLocation(i, 0))
            f1.Controls(i).Font = FontSize(f1.Controls(i).Font.Name, fontContainer(i, 0), ControlSizeLocation(i, 1), f1.Controls(i).Location)
            'ControlArray(i) = f1.Controls(i) 
            'f1.Controls.Remove(f1.Controls(0)) 
            If IsContainer(f1.Controls(i)) Then
                For m As UInt16 = 0 To f1.Controls(i).Controls.Count - 1
                    f1.Controls(i).Controls(m).Location = ReLocater(CType(SizeLocationContainer(m, 1, n), Point))
                    f1.Controls(i).Controls(m).Size = ReSizer(CType(SizeLocationContainer(m, 0, n), Point))
                    f1.Controls(i).Controls(m).Font = FontSize(f1.Controls(i).Controls(m).Font.Name, fontContainer(m, n + 1), SizeLocationContainer(m, 1, n), f1.Controls(i).Controls(m).Location)
                Next
                n += 1
            End If
        Next

    End Sub

    Private Function ReLocater(ByVal p As Point) As Point
        Dim OldLocationX As UInt32 = p.X
        Dim OldLocationY As UInt32 = p.Y
        Dim NewLocationX As UInt32 = CInt((OldLocationX * NewWidthForm) / OriginalWidthForm)
        Dim NewLocationY As UInt32 = CInt((OldLocationY * NewHeightForm) / OriginalHeightForm)
        ReLocater = New Point(NewLocationX, NewLocationY)
    End Function

    Private Function ReSizer(ByVal p As Point) As Point
        Dim OldWidthControl As UInt32 = p.X
        Dim OldHeightControl As UInt32 = p.Y
        Dim NewWidthControl As UInt32 = CInt((OldWidthControl * NewWidthForm) / OriginalWidthForm)
        Dim NewHeightControl As UInt32 = CInt((OldHeightControl * NewHeightForm) / OriginalHeightForm)
        Return New Point(NewWidthControl, NewHeightControl)
    End Function

    Private Function IsContainer(ByVal con As Control) As Boolean
        If con.GetType.Name = "TableLayoutPanel" Or
            con.GetType.Name = "GroupBox" Or
            con.GetType.Name = "Panel" Or
            con.GetType.Name = "FlowLayoutPanel" Or
            con.GetType.Name = "SplitContainer" Or
            con.GetType.Name = "TabControl" Then
            Return True
        End If
        Return False
    End Function

    Private Function FontSize(ByVal FontName As String, ByVal OrginalFontSize As UInt16, ByVal OldLoc As Point, ByVal NewLoc As Point) As Font
        Dim ns As UInt16
        ns = 0.75 * OrginalFontSize * NewLoc.X / OldLoc.X
        Dim f1 As New Font(FontName, ns)
        Return f1
    End Function

End Class