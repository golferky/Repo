Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace HughsGolf.Tests
    <TestClass>
    Public Class LeagueDateServiceTests
        <TestMethod>
        Sub GetFrontBack_StartsOnFront_OddMonthReturnsBack()
            Dim result = LeagueDateService.GetFrontBack("20250506", "F")

            Assert.AreEqual("Back", result)
        End Sub

        <TestMethod>
        Sub GetFrontBack_StartsOnFront_EvenMonthReturnsFront()
            Dim result = LeagueDateService.GetFrontBack("20250603", "F")

            Assert.AreEqual("Front", result)
        End Sub

        <TestMethod>
        Sub GetFrontBack_StartsOnBack_OddMonthReturnsFront()
            Dim result = LeagueDateService.GetFrontBack("20250506", "B")

            Assert.AreEqual("Front", result)
        End Sub

        <TestMethod>
        Sub GetFrontBack_StartsOnBack_EvenMonthReturnsBack()
            Dim result = LeagueDateService.GetFrontBack("20250603", "B")

            Assert.AreEqual("Back", result)
        End Sub

        <TestMethod>
        Sub GetFrontBack_RainoutOverride_ReturnsBack()
            Dim result = LeagueDateService.GetFrontBack("20251007", "F")

            Assert.AreEqual("Back", result)
        End Sub
    End Class
End Namespace

