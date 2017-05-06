Imports Newtonsoft.Json
Imports System.IO

Module Module1

	Sub Main(ByVal args As String())
        Try
            Dim Data As Integer() = GetData(args)
            Console.WriteLine(Bonus(Data))
        Catch ex As ArgumentException
            Console.WriteLine("Please specify a filename")
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException _
            OrElse TypeOf ex Is Security.SecurityException
            Console.WriteLine("Could not read file")
        Catch ex As NotSupportedException
            Console.WriteLine("Invalid path")
        Catch ex As JsonReaderException
            Console.WriteLine("Bad file format")
		End Try
		Console.ReadKey()
	End Sub

    Private Function NoBonus(ByVal data As Integer()) As Boolean
        Dim ForwardIndex As Integer = 0
        Dim BackwardIndex As Integer = data.Length - 1

        While ForwardIndex < BackwardIndex
            Dim Lesser As Integer = data(ForwardIndex)
            Dim Greater As Integer = data(BackwardIndex)

            If Lesser + Greater = 0 OrElse Lesser = 0 OrElse Greater = 0 Then
                Return True
            ElseIf Lesser + Greater > 0 Then
                BackwardIndex -= 1
            ElseIf Lesser + Greater < 0 Then
                ForwardIndex += 1
            End If
        End While

        Return False
    End Function

    Private Function Bonus(ByVal data As Integer()) As Boolean
        If data.Length = 0 Then
            Return False
        End If

        If data(0) > 0 OrElse data(data.Length - 1) < 0 Then
            Return False
        End If

        If Array.Exists(data, Function(element) element = 0) Then
            Return True
        End If

        Dim Negatives As New HashSet(Of Integer)
        Dim i As Integer = 0
        While data(i) < 0
            Dim AbsoluteValue As Integer = -data(i)
            For Each Negative As Integer In Negatives.ToArray()
                Negatives.Add(AbsoluteValue + Negative)
            Next
            Negatives.Add(AbsoluteValue)
            i += 1
        End While

        Dim Positives As New HashSet(Of Integer)
        While i < data.Length
            For Each Positive As Integer In Positives.ToArray()
                Positives.Add(data(i) + Positive)
            Next
            Positives.Add(data(i))
            i += 1
        End While

        Return Negatives.Overlaps(Positives)
    End Function

    Private Function GetData(ByVal args As String()) As Integer()
		If args.Length < 1 Then
            Throw New ArgumentNullException()
        End If

		Dim FileContents As String = File.ReadAllText(args(0))

		Dim Data As Linq.JArray = CType(JsonConvert.DeserializeObject(FileContents), Linq.JArray)
		Dim ConvertedData(Data.Count - 1) As Integer
		For i As Integer = 0 To Data.Count - 1
			ConvertedData(i) = Data(i).ToObject(Of Integer)
		Next

		Return ConvertedData
	End Function

End Module
