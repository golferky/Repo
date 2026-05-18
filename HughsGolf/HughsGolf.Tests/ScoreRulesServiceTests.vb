Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace HughsGolf.Tests
    <TestClass>
    Public Class ScoreRulesServiceTests
        <TestMethod>
        Sub CalculateGross_AllNineScores_ReturnsTotal()
            Dim result = ScoreRulesService.CalculateGross({4, 5, 3, 4, 6, 5, 4, 3, 5})

            Assert.AreEqual(39, result)
        End Sub

        <TestMethod>
        Sub CalculateGross_BlanksAndNonNumericValues_AreIgnored()
            Dim result = ScoreRulesService.CalculateGross({4, DBNull.Value, Nothing, "x", 5})

            Assert.AreEqual(9, result)
        End Sub

        <TestMethod>
        Sub CalculateGross_NoScores_ReturnsZero()
            Dim result = ScoreRulesService.CalculateGross({DBNull.Value, Nothing, ""})

            Assert.AreEqual(0, result)
        End Sub

        <TestMethod>
        Sub CalculateNet_SubtractsHandicapFromGross()
            Dim result = ScoreRulesService.CalculateNet(42, 7)

            Assert.AreEqual(35, result)
        End Sub

        <TestMethod>
        Sub CtpCarryoverDetail_UsesSlotNumberNotHoleNumber()
            Dim result = ScoreRulesService.CtpCarryoverDetail(1, "Back")

            Assert.AreEqual("Carryover1-Back", result)
        End Sub

        <TestMethod>
        Sub CapScore_Par3AboveMax_ReturnsPar3Max()
            Dim result = ScoreRulesService.CapScore(8, 3, 6, 8, 10)

            Assert.AreEqual(6D, result)
        End Sub

        <TestMethod>
        Sub CapScore_Par4AboveMax_ReturnsPar4Max()
            Dim result = ScoreRulesService.CapScore(10, 4, 6, 8, 10)

            Assert.AreEqual(8D, result)
        End Sub

        <TestMethod>
        Sub CapScore_Par5AboveMax_ReturnsPar5Max()
            Dim result = ScoreRulesService.CapScore(12, 5, 6, 8, 10)

            Assert.AreEqual(10D, result)
        End Sub

        <TestMethod>
        Sub CapScore_AtMax_ReturnsOriginalScore()
            Dim result = ScoreRulesService.CapScore(8, 4, 6, 8, 10)

            Assert.AreEqual(8D, result)
        End Sub

        <TestMethod>
        Sub CapScore_BelowMax_ReturnsOriginalScore()
            Dim result = ScoreRulesService.CapScore(5, 4, 6, 8, 10)

            Assert.AreEqual(5D, result)
        End Sub

        <TestMethod>
        Sub CapScore_UnknownParUsesFallbackMax()
            Dim result = ScoreRulesService.CapScore(120, 6, 6, 8, 10)

            Assert.AreEqual(99D, result)
        End Sub
    End Class
End Namespace
