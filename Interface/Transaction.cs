class TransactionStatus
{
  public const String PROCESSING = "PROCESSING";
  public const String SUCCESS = "SUCCESS";
  public const String REFUSE = "REFUSE";
}

public class CreateTransactionBody
{
  public String storeId { get; set; }
  public String offerId { get; set; }
}

public class ConfirmTransaction
{
  public String? transactionId { get; set; }
  public String status { get; set; }
}