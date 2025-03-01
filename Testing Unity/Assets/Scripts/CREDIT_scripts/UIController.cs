using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Borrower Profile UI")]
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI creditScoreText;
    public TextMeshProUGUI riskLevelText;
    public TextMeshProUGUI loanAmountText;

    [Header("Game State UI")]
    public TextMeshProUGUI currentFundsText;
    public TextMeshProUGUI roundCountText;
    public TextMeshProUGUI messageText;
    public TMP_InputField interestRateInput;
    public Button approveLoanButton;
    public Button rejectLoanButton;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public Button restartButton;

    private LoanManager loanManager;

    private void Start()
    {
        loanManager = FindFirstObjectByType<LoanManager>();
        SetupEventListeners();
        InitializeUI();
    }

    private void SetupEventListeners()
    {
        if (loanManager != null)
        {
            loanManager.onFundsUpdated.AddListener(UpdateFundsDisplay);
            loanManager.onNewBorrower.AddListener(UpdateBorrowerDisplay);
            loanManager.onGameMessage.AddListener(DisplayMessage);
            loanManager.onGameOver.AddListener(ShowGameOver);
        }

        if (approveLoanButton != null)
        {
            approveLoanButton.onClick.AddListener(OnApproveLoanClicked);
        }

        if (rejectLoanButton != null)
        {
            rejectLoanButton.onClick.AddListener(OnRejectLoanClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
        }
    }

    private void InitializeUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (interestRateInput != null)
        {
            interestRateInput.contentType = TMP_InputField.ContentType.DecimalNumber;
            interestRateInput.text = "0.10";
        }

        UpdateFundsDisplay(loanManager.currentFunds);
        UpdateRoundDisplay();
    }

    public void UpdateBorrowerDisplay(NPCBorrower borrower)
    {
        if (borrower == null) return;

        if (incomeText != null)
            incomeText.text = $"Annual Income: {NPCBorrower.FormatCurrency(borrower.income)}";
        
        if (debtText != null)
            debtText.text = $"Current Debt: {NPCBorrower.FormatCurrency(borrower.debt)}";
        
        if (creditScoreText != null)
            creditScoreText.text = $"Credit Score: {borrower.creditScore}";
        
        if (riskLevelText != null)
            riskLevelText.text = $"Risk Level: {borrower.GetRiskLevelString()}";
        
        if (loanAmountText != null)
            loanAmountText.text = $"Requested Loan: {NPCBorrower.FormatCurrency(borrower.requestedLoanAmount)}";

        UpdateRoundDisplay();
        EnableLoanButtons(true);
    }

    public void UpdateFundsDisplay(float amount)
    {
        if (currentFundsText != null)
        {
            currentFundsText.text = $"Available Funds: {NPCBorrower.FormatCurrency(amount)}";
        }
    }

    private void UpdateRoundDisplay()
    {
        if (roundCountText != null)
        {
            roundCountText.text = $"Round: {loanManager.currentRound}/{loanManager.maxRounds}";
        }
    }

    public void DisplayMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    private void OnApproveLoanClicked()
    {
        if (!loanManager.CanProcessLoan()) return;

        float interestRate;
        if (float.TryParse(interestRateInput.text, out interestRate))
        {
            interestRate = Mathf.Clamp(interestRate, 0f, 1f);
            loanManager.ProcessLoanDecision(true, interestRate);
            EnableLoanButtons(false);
        }
        else
        {
            DisplayMessage("Please enter a valid interest rate (0-1)");
        }
    }

    private void OnRejectLoanClicked()
    {
        if (!loanManager.CanProcessLoan()) return;

        loanManager.ProcessLoanDecision(false, 0f);
        EnableLoanButtons(false);
    }

    private void EnableLoanButtons(bool enable)
    {
        if (approveLoanButton != null)
            approveLoanButton.interactable = enable;
        
        if (rejectLoanButton != null)
            rejectLoanButton.interactable = enable;
        
        if (interestRateInput != null)
            interestRateInput.interactable = enable;
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            float profit = loanManager.currentFunds - loanManager.startingFunds;
            string profitText = profit >= 0 ? "Profit" : "Loss";
            finalScoreText.text = $"Final Balance: {NPCBorrower.FormatCurrency(loanManager.currentFunds)}\n" +
                                $"Total {profitText}: {NPCBorrower.FormatCurrency(Mathf.Abs(profit))}";
        }

        EnableLoanButtons(false);
    }
} 