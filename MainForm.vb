Imports System.Data.SqlClient
Imports System.Windows.Forms

Public Class MainForm
    Inherits Form
    
    Private dbManager As DatabaseManager
    Private currentCustomerId As Integer = 0
    
    ' Form controls as class-level variables
    Private txtDate As TextBox
    Private txtName As TextBox
    Private txtAddress As TextBox
    Private txtPhone As TextBox
    Private txtEmail As TextBox
    Private txtCamperYear As TextBox
    Private txtCamperMake As TextBox
    Private txtCamperModel As TextBox
    Private txtComments As TextBox
    Private lstCustomers As ListBox

    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Customer & Camper Management System"
        Me.Size = New Size(1000, 700)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.WhiteSmoke
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            dbManager = New DatabaseManager()
            dbManager.InitializeDatabase()

            SetupUI()
            LoadCustomers()
        Catch ex As Exception
            MessageBox.Show("Error initializing application: " & ex.Message)
        End Try
    End Sub

    Private Sub SetupUI()
        ' Main TableLayout
        Dim mainLayout As New TableLayoutPanel
        mainLayout.Dock = DockStyle.Fill
        mainLayout.ColumnCount = 2
        mainLayout.RowCount = 1
        mainLayout.ColumnStyles.Clear()
        mainLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30))
        mainLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 70))

        ' Left Panel - Customer List
        Dim leftPanel As New Panel
        leftPanel.Dock = DockStyle.Fill
        leftPanel.BorderStyle = BorderStyle.FixedSingle
        leftPanel.BackColor = Color.White
        leftPanel.Padding = New Padding(10)

        Dim lblTitle As New Label
        lblTitle.Text = "CUSTOMER LIST"
        lblTitle.Font = New Font("Arial", 12, FontStyle.Bold)
        lblTitle.AutoSize = True
        lblTitle.Margin = New Padding(0, 0, 0, 10)
        leftPanel.Controls.Add(lblTitle)

        lstCustomers = New ListBox
        lstCustomers.Dock = DockStyle.Fill
        lstCustomers.Top = lblTitle.Bottom + 10
        AddHandler lstCustomers.SelectedIndexChanged, AddressOf lstCustomers_SelectedIndexChanged
        leftPanel.Controls.Add(lstCustomers)

        ' Right Panel - Form Fields
        Dim rightPanel As New Panel
        rightPanel.Dock = DockStyle.Fill
        rightPanel.AutoScroll = True
        rightPanel.Padding = New Padding(15)

        Dim formLayout As New TableLayoutPanel
        formLayout.Dock = DockStyle.Top
        formLayout.AutoSize = True
        formLayout.ColumnCount = 2
        formLayout.RowCount = 11
        formLayout.ColumnStyles.Clear()
        formLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 120))
        formLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        formLayout.Padding = New Padding(10)

        ' Add Form Controls
        Dim row As Integer = 0

        ' Date
        formLayout.Controls.Add(CreateLabel("Date:"), 0, row)
        txtDate = New TextBox
        txtDate.Text = Now.ToShortDateString()
        txtDate.ReadOnly = True
        formLayout.Controls.Add(txtDate, 1, row)

        ' Name
        row += 1
        formLayout.Controls.Add(CreateLabel("Name:"), 0, row)
        txtName = New TextBox
        formLayout.Controls.Add(txtName, 1, row)

        ' Address
        row += 1
        formLayout.Controls.Add(CreateLabel("Address:"), 0, row)
        txtAddress = New TextBox
        formLayout.Controls.Add(txtAddress, 1, row)

        ' Phone
        row += 1
        formLayout.Controls.Add(CreateLabel("Phone:"), 0, row)
        txtPhone = New TextBox
        formLayout.Controls.Add(txtPhone, 1, row)

        ' Email
        row += 1
        formLayout.Controls.Add(CreateLabel("Email:"), 0, row)
        txtEmail = New TextBox
        formLayout.Controls.Add(txtEmail, 1, row)

        ' Camper Year
        row += 1
        formLayout.Controls.Add(CreateLabel("Camper Year:"), 0, row)
        txtCamperYear = New TextBox
        formLayout.Controls.Add(txtCamperYear, 1, row)

        ' Camper Make
        row += 1
        formLayout.Controls.Add(CreateLabel("Camper Make:"), 0, row)
        txtCamperMake = New TextBox
        formLayout.Controls.Add(txtCamperMake, 1, row)

        ' Camper Model
        row += 1
        formLayout.Controls.Add(CreateLabel("Camper Model:"), 0, row)
        txtCamperModel = New TextBox
        formLayout.Controls.Add(txtCamperModel, 1, row)

        ' Comments
        row += 1
        formLayout.Controls.Add(CreateLabel("Comments:"), 0, row)
        txtComments = New TextBox
        txtComments.Multiline = True
        txtComments.Height = 80
        formLayout.Controls.Add(txtComments, 1, row)

        ' Buttons Panel
        row += 1
        Dim btnPanel As New Panel
        btnPanel.Height = 40
        btnPanel.Dock = DockStyle.Top

        Dim btnNew As New Button
        btnNew.Text = "NEW"
        btnNew.Width = 80
        btnNew.Location = New Point(10, 5)
        AddHandler btnNew.Click, AddressOf btnNew_Click
        btnPanel.Controls.Add(btnNew)

        Dim btnSave As New Button
        btnSave.Text = "SAVE"
        btnSave.Width = 80
        btnSave.Location = New Point(100, 5)
        AddHandler btnSave.Click, AddressOf btnSave_Click
        btnPanel.Controls.Add(btnSave)

        Dim btnDelete As New Button
        btnDelete.Text = "DELETE"
        btnDelete.Width = 80
        btnDelete.Location = New Point(190, 5)
        AddHandler btnDelete.Click, AddressOf btnDelete_Click
        btnPanel.Controls.Add(btnDelete)

        Dim btnPrint As New Button
        btnPrint.Text = "PRINT"
        btnPrint.Width = 80
        btnPrint.Location = New Point(280, 5)
        AddHandler btnPrint.Click, AddressOf btnPrint_Click
        btnPanel.Controls.Add(btnPrint)

        Dim btnSearch As New Button
        btnSearch.Text = "SEARCH"
        btnSearch.Width = 80
        btnSearch.Location = New Point(370, 5)
        AddHandler btnSearch.Click, AddressOf btnSearch_Click
        btnPanel.Controls.Add(btnSearch)

        formLayout.Controls.Add(btnPanel, 0, row)
        formLayout.SetColumnSpan(btnPanel, 2)

        rightPanel.Controls.Add(formLayout)
        mainLayout.Controls.Add(leftPanel, 0, 0)
        mainLayout.Controls.Add(rightPanel, 1, 0)

        Me.Controls.Add(mainLayout)
    End Sub

    Private Function CreateLabel(text As String) As Label
        Dim lbl As New Label
        lbl.Text = text
        lbl.AutoSize = True
        lbl.Font = New Font("Arial", 10, FontStyle.Regular)
        Return lbl
    End Function

    Private Sub LoadCustomers()
        Try
            Dim customers = dbManager.GetAllCustomers()
            lstCustomers.Items.Clear()

            For Each cust In customers
                lstCustomers.Items.Add(cust.CustomerID & " - " & cust.Name & " (" & cust.Date.ToShortDateString() & ")")
            Next
        Catch ex As Exception
            MessageBox.Show("Error loading customers: " & ex.Message)
        End Try
    End Sub

    Private Sub lstCustomers_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            If lstCustomers.SelectedIndex >= 0 Then
                Dim selectedText = lstCustomers.SelectedItem.ToString()
                Dim customerId = Integer.Parse(selectedText.Split("-")(0).Trim())
                currentCustomerId = customerId

                Dim customer = dbManager.GetCustomer(customerId)
                If customer IsNot Nothing Then
                    PopulateForm(customer)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub PopulateForm(customer As Customer)
        txtDate.Text = customer.Date.ToShortDateString()
        txtName.Text = customer.Name
        txtAddress.Text = customer.Address
        txtPhone.Text = customer.Phone
        txtEmail.Text = customer.Email
        txtCamperYear.Text = customer.CamperYear
        txtCamperMake.Text = customer.CamperMake
        txtCamperModel.Text = customer.CamperModel
        txtComments.Text = customer.Comments
    End Sub

    Private Sub btnNew_Click(sender As Object, e As EventArgs)
        currentCustomerId = 0
        ClearForm()
    End Sub

    Private Sub ClearForm()
        txtDate.Text = Now.ToShortDateString()
        txtName.Text = ""
        txtAddress.Text = ""
        txtPhone.Text = ""
        txtEmail.Text = ""
        txtCamperYear.Text = ""
        txtCamperMake.Text = ""
        txtCamperModel.Text = ""
        txtComments.Text = ""
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
            If txtName.Text = "" Then
                MessageBox.Show("Please enter a customer name.")
                Return
            End If

            Dim customer As New Customer
            customer.CustomerID = currentCustomerId
            customer.Date = CDate(txtDate.Text)
            customer.Name = txtName.Text
            customer.Address = txtAddress.Text
            customer.Phone = txtPhone.Text
            customer.Email = txtEmail.Text
            customer.CamperYear = txtCamperYear.Text
            customer.CamperMake = txtCamperMake.Text
            customer.CamperModel = txtCamperModel.Text
            customer.Comments = txtComments.Text

            If currentCustomerId = 0 Then
                dbManager.AddCustomer(customer)
                MessageBox.Show("Customer added successfully!")
            Else
                dbManager.UpdateCustomer(customer)
                MessageBox.Show("Customer updated successfully!")
            End If

            LoadCustomers()
            ClearForm()
        Catch ex As Exception
            MessageBox.Show("Error saving customer: " & ex.Message)
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs)
        Try
            If currentCustomerId = 0 Then
                MessageBox.Show("Please select a customer to delete.")
                Return
            End If

            If MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                dbManager.DeleteCustomer(currentCustomerId)
                MessageBox.Show("Customer deleted successfully!")
                LoadCustomers()
                ClearForm()
                currentCustomerId = 0
            End If
        Catch ex As Exception
            MessageBox.Show("Error deleting customer: " & ex.Message)
        End Try
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs)
        Try
            If currentCustomerId > 0 Then
                Dim customer = dbManager.GetCustomer(currentCustomerId)
                PrintCustomer(customer)
            Else
                MessageBox.Show("Please select a customer to print.")
            End If
        Catch ex As Exception
            MessageBox.Show("Error printing: " & ex.Message)
        End Try
    End Sub

    Private Sub PrintCustomer(customer As Customer)
        Dim pd As New Printing.PrintDocument
        AddHandler pd.PrintPage, Sub(s, e)
                                      Dim font As New Font("Arial", 10)
                                      Dim y As Integer = 50

                                      e.Graphics.DrawString("CUSTOMER INFORMATION REPORT", New Font("Arial", 14, FontStyle.Bold), Brushes.Black, 50, y)
                                      y += 40

                                      e.Graphics.DrawString("Customer ID: " & customer.CustomerID, font, Brushes.Black, 50, y)
                                      y += 20
                                      e.Graphics.DrawString("Date: " & customer.Date.ToShortDateString(), font, Brushes.Black, 50, y)
                                      y += 20
                                      e.Graphics.DrawString("Name: " & customer.Name, font, Brushes.Black, 50, y)
                                      y += 20
                                      e.Graphics.DrawString("Address: " & customer.Address, font, Brushes.Black, 50, y)
                                      y += 20
                                      e.Graphics.DrawString("Phone: " & customer.Phone, font, Brushes.Black, 50, y)
                                      y += 20
                                      e.Graphics.DrawString("Email: " & customer.Email, font, Brushes.Black, 50, y)
                                      y += 20
                                      e.Graphics.DrawString("Camper Year: " & customer.CamperYear, font, Brushes.Black, 50, y)
                                      y += 20
                                      e.Graphics.DrawString("Camper Make: " & customer.CamperMake, font, Brushes.Black, 50, y)
                                      y += 20
                                      e.Graphics.DrawString("Camper Model: " & customer.CamperModel, font, Brushes.Black, 50, y)
                                      y += 20
                                      e.Graphics.DrawString("Comments: " & customer.Comments, font, Brushes.Black, 50, y)
                                  End Sub

        Dim pv As New PrintPreviewDialog
        pv.Document = pd
        pv.ShowDialog()
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs)
        Dim searchForm As New SearchForm(dbManager)
        searchForm.ShowDialog()
        LoadCustomers()
    End Sub
End Class