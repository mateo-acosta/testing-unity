using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class LoanManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float startingFunds = 100000f;
    public int maxRounds = 20;
    public float timeBetweenRounds = 2f;

    [Header("Current Game State")]
    public float currentFunds;
    public int currentRound = 0;
    public NPCBorrower currentBorrower;

    [Header("Events")]
    public UnityEvent<float> onFundsUpdated;
    public UnityEvent<NPCBorrower> onNewBorrower;
    public UnityEvent<string> onGameMessage;
    public UnityEvent onGameOver;

    private bool isProcessingLoan = false;

    private void Start()
    {
        currentFunds = startingFunds;
        StartNextRound();
    }

    public void ProcessLoanDecision(bool approved, float interestRate)
    {
        if (isProcessingLoan || currentBorrower == null) return;
        
        StartCoroutine(ProcessLoanDecisionCoroutine(approved, interestRate));
    }

    private IEnumerator ProcessLoanDecisionCoroutine(bool approved, float interestRate)
    {
        isProcessingLoan = true;

        if (!approved)
        {
            onGameMessage.Invoke("Loan rejected by lender.");
            yield return new WaitForSeconds(timeBetweenRounds);
            StartNextRound();
            isProcessingLoan = false;
            yield break;
        }

        // Check if we have sufficient funds
        if (currentFunds < currentBorrower.requestedLoanAmount)
        {
            onGameMessage.Invoke("Insufficient funds to issue loan!");
            yield return new WaitForSeconds(timeBetweenRounds);
            StartNextRound();
            isProcessingLoan = false;
            yield break;
        }

        // Check if borrower accepts the interest rate
        if (!currentBorrower.WillAcceptInterestRate(interestRate))
        {
            onGameMessage.Invoke("Borrower rejected the offered interest rate.");
            yield return new WaitForSeconds(timeBetweenRounds);
            StartNextRound();
            isProcessingLoan = false;
            yield break;
        }

        // Issue the loan
        float loanAmount = currentBorrower.requestedLoanAmount;
        currentFunds -= loanAmount;
        onFundsUpdated.Invoke(currentFunds);
        onGameMessage.Invoke("Loan issued. Waiting for repayment period...");

        yield return new WaitForSeconds(timeBetweenRounds);

        // Simulate repayment
        if (currentBorrower.SimulateRepayment())
        {
            // Calculate return with interest
            float interestAmount = loanAmount * interestRate;
            float totalReturn = loanAmount + interestAmount;
            currentFunds += totalReturn;
            onFundsUpdated.Invoke(currentFunds);
            onGameMessage.Invoke($"Loan repaid with {interestRate:P0} interest! Profit: {NPCBorrower.FormatCurrency(interestAmount)}");
        }
        else
        {
            onGameMessage.Invoke("Borrower defaulted on the loan! Principal lost.");
        }

        yield return new WaitForSeconds(timeBetweenRounds);
        
        // Check if game should continue
        if (currentRound >= maxRounds || currentFunds <= 0)
        {
            EndGame();
        }
        else
        {
            StartNextRound();
        }

        isProcessingLoan = false;
    }

    private void StartNextRound()
    {
        currentRound++;
        
        // Create new borrower
        if (currentBorrower == null)
        {
            GameObject borrowerObj = new GameObject("Current Borrower");
            currentBorrower = borrowerObj.AddComponent<NPCBorrower>();
        }
        
        currentBorrower.GenerateProfile();
        onNewBorrower.Invoke(currentBorrower);
    }

    private void EndGame()
    {
        string endMessage = currentFunds <= 0 
            ? "Game Over - You've run out of funds!" 
            : $"Game Complete! Final Balance: {NPCBorrower.FormatCurrency(currentFunds)}";
        
        onGameMessage.Invoke(endMessage);
        onGameOver.Invoke();
    }

    public bool CanProcessLoan()
    {
        return !isProcessingLoan && currentBorrower != null;
    }
} 