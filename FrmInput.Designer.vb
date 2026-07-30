<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmInput
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        OpenFileDialog1 = New OpenFileDialog()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        txtFileName = New TextBox()
        btnFile = New Button()
        MonthCalendar1 = New MonthCalendar()
        DataGridView1 = New DataGridView()
        txtStartDate = New TextBox()
        txtEndDate = New TextBox()
        Label4 = New Label()
        MonthCalendar2 = New MonthCalendar()
        txtPromoPeriod = New TextBox()
        btnSave = New Button()
        ProgressBar1 = New ProgressBar()
        Label5 = New Label()
        txtTotalRec = New TextBox()
        DataGridView2 = New DataGridView()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        CType(DataGridView2, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' OpenFileDialog1
        ' 
        OpenFileDialog1.FileName = "OpenFileDialog1"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(14, 17)
        Label1.Name = "Label1"
        Label1.Size = New Size(76, 20)
        Label1.TabIndex = 1
        Label1.Text = "Start Date"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(632, 14)
        Label2.Name = "Label2"
        Label2.Size = New Size(74, 20)
        Label2.TabIndex = 2
        Label2.Text = "End Date "
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(14, 347)
        Label3.Name = "Label3"
        Label3.Size = New Size(69, 20)
        Label3.TabIndex = 4
        Label3.Text = "Filename"
        ' 
        ' txtFileName
        ' 
        txtFileName.BackColor = SystemColors.ActiveBorder
        txtFileName.BorderStyle = BorderStyle.FixedSingle
        txtFileName.Location = New Point(142, 344)
        txtFileName.Margin = New Padding(3, 4, 3, 4)
        txtFileName.Name = "txtFileName"
        txtFileName.Size = New Size(678, 27)
        txtFileName.TabIndex = 5
        ' 
        ' btnFile
        ' 
        btnFile.Location = New Point(826, 340)
        btnFile.Margin = New Padding(3, 4, 3, 4)
        btnFile.Name = "btnFile"
        btnFile.Size = New Size(34, 31)
        btnFile.TabIndex = 6
        btnFile.Text = "..."
        btnFile.UseVisualStyleBackColor = True
        ' 
        ' MonthCalendar1
        ' 
        MonthCalendar1.Location = New Point(14, 49)
        MonthCalendar1.Margin = New Padding(10, 12, 10, 12)
        MonthCalendar1.Name = "MonthCalendar1"
        MonthCalendar1.TabIndex = 7
        ' 
        ' DataGridView1
        ' 
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Location = New Point(14, 381)
        DataGridView1.Margin = New Padding(3, 4, 3, 4)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.RowHeadersWidth = 51
        DataGridView1.Size = New Size(559, 400)
        DataGridView1.TabIndex = 9
        ' 
        ' txtStartDate
        ' 
        txtStartDate.BackColor = SystemColors.Control
        txtStartDate.Location = New Point(87, 13)
        txtStartDate.Margin = New Padding(3, 4, 3, 4)
        txtStartDate.Name = "txtStartDate"
        txtStartDate.Size = New Size(156, 27)
        txtStartDate.TabIndex = 10
        ' 
        ' txtEndDate
        ' 
        txtEndDate.BackColor = SystemColors.Control
        txtEndDate.Location = New Point(704, 10)
        txtEndDate.Margin = New Padding(3, 4, 3, 4)
        txtEndDate.Name = "txtEndDate"
        txtEndDate.Size = New Size(156, 27)
        txtEndDate.TabIndex = 11
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(14, 279)
        Label4.Name = "Label4"
        Label4.Size = New Size(125, 20)
        Label4.TabIndex = 12
        Label4.Text = "Promotion Period"
        ' 
        ' MonthCalendar2
        ' 
        MonthCalendar2.Location = New Point(601, 46)
        MonthCalendar2.Margin = New Padding(10, 12, 10, 12)
        MonthCalendar2.Name = "MonthCalendar2"
        MonthCalendar2.TabIndex = 13
        ' 
        ' txtPromoPeriod
        ' 
        txtPromoPeriod.BackColor = SystemColors.ActiveBorder
        txtPromoPeriod.BorderStyle = BorderStyle.FixedSingle
        txtPromoPeriod.Location = New Point(142, 275)
        txtPromoPeriod.Margin = New Padding(3, 4, 3, 4)
        txtPromoPeriod.Name = "txtPromoPeriod"
        txtPromoPeriod.Size = New Size(718, 27)
        txtPromoPeriod.TabIndex = 14
        ' 
        ' btnSave
        ' 
        btnSave.Location = New Point(14, 793)
        btnSave.Margin = New Padding(3, 4, 3, 4)
        btnSave.Name = "btnSave"
        btnSave.Size = New Size(86, 31)
        btnSave.TabIndex = 15
        btnSave.Text = "Save"
        btnSave.UseVisualStyleBackColor = True
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Location = New Point(106, 796)
        ProgressBar1.Margin = New Padding(3, 4, 3, 4)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(467, 28)
        ProgressBar1.TabIndex = 16
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(703, 801)
        Label5.Name = "Label5"
        Label5.Size = New Size(99, 20)
        Label5.TabIndex = 17
        Label5.Text = "Total Records"
        ' 
        ' txtTotalRec
        ' 
        txtTotalRec.BackColor = SystemColors.Control
        txtTotalRec.Font = New Font("Segoe UI", 9F, FontStyle.Bold)
        txtTotalRec.Location = New Point(792, 797)
        txtTotalRec.Margin = New Padding(3, 4, 3, 4)
        txtTotalRec.Name = "txtTotalRec"
        txtTotalRec.Size = New Size(68, 27)
        txtTotalRec.TabIndex = 18
        ' 
        ' DataGridView2
        ' 
        DataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView2.Location = New Point(593, 381)
        DataGridView2.Margin = New Padding(3, 4, 3, 4)
        DataGridView2.Name = "DataGridView2"
        DataGridView2.RowHeadersWidth = 51
        DataGridView2.Size = New Size(267, 400)
        DataGridView2.TabIndex = 19
        ' 
        ' FrmInput
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(872, 831)
        Controls.Add(DataGridView2)
        Controls.Add(txtTotalRec)
        Controls.Add(Label5)
        Controls.Add(ProgressBar1)
        Controls.Add(btnSave)
        Controls.Add(txtPromoPeriod)
        Controls.Add(MonthCalendar2)
        Controls.Add(Label4)
        Controls.Add(txtEndDate)
        Controls.Add(txtStartDate)
        Controls.Add(DataGridView1)
        Controls.Add(MonthCalendar1)
        Controls.Add(btnFile)
        Controls.Add(txtFileName)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Margin = New Padding(3, 4, 3, 4)
        MaximizeBox = False
        MaximumSize = New Size(890, 878)
        MinimumSize = New Size(890, 878)
        Name = "FrmInput"
        StartPosition = FormStartPosition.CenterScreen
        Text = "PDU {PriceTag Data Upload}  - "
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        CType(DataGridView2, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtFileName As TextBox
    Friend WithEvents btnFile As Button
    Friend WithEvents MonthCalendar1 As MonthCalendar
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents txtStartDate As TextBox
    Friend WithEvents txtEndDate As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents MonthCalendar2 As MonthCalendar
    Friend WithEvents txtPromoPeriod As TextBox
    Friend WithEvents btnSave As Button
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Label5 As Label
    Friend WithEvents txtTotalRec As TextBox
    Friend WithEvents DataGridView2 As DataGridView

End Class
