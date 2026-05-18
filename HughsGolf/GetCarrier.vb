Imports System.Net.Http
Imports System.Reflection.Emit
Imports Newtonsoft.Json.Linq

Public Class GetCarrier
    Public Async Function GetCarrierInfo(phoneNumber As String) As Task(Of String)
        Dim apiUrl As String = $"https://api.numlookupapi.com/v1/lookup/{phoneNumber}"
        Dim apiKey As String = $"v1/ validate / +1{phoneNumber}?apikey=num_live_Ke7RP2ZoVfYsDGarCDkeO9KMXpWdSy0Y4XRzeJq6"

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("X-API-Key", apiKey)

            Dim response As HttpResponseMessage = Await client.GetAsync(apiUrl)

            If response.IsSuccessStatusCode Then
                Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                Dim jsonObj As JObject = JObject.Parse(jsonResponse)
                Dim carrier As String = jsonObj("carrier").ToString()

                Return carrier
            Else
                Return "Error retrieving carrier information."
            End If
        End Using
    End Function
End Class
