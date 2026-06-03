using Domain.SharedKernel;

namespace Domain.Claims.Errors;

public static class ClaimErrors
{
    public static Error EmptyDescription    = new("Claim.EmptyDescription",    "Claim description is required.");
    public static Error InvalidAmount       = new("Claim.InvalidAmount",       "Claim amount must be greater than zero.");
    public static Error DocumentTooLarge    = new("Claim.DocumentTooLarge",    "Document size cannot exceed 10MB.");
    public static Error AlreadyApproved     = new("Claim.AlreadyApproved",     "Claim is already approved.");
    public static Error AlreadyRejected     = new("Claim.AlreadyRejected",     "Claim is already rejected.");
    public static Error EmptyRejectionReason= new("Claim.EmptyRejectionReason","Rejection reason is required.");
    public static Error ApprovedExceedsClaim= new("Claim.ApprovedExceedsClaim","Approved amount cannot exceed claimed amount.");
    public static Error NotFound            = new("Claim.NotFound",            "Claim was not found.");
    public static Error InvalidItemQuantity = new("Claim.InvalidItemQuantity", "Item quantity must be greater than zero.");
    public static Error EmptyItemDescription= new("Claim.EmptyItemDescription","Item description is required.");
}
