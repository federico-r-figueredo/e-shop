# Problems

## Problem #1: OrderingContext's current txn is null

### Symptom

IntegrationEventLogService threw `ArgumentNullException: Value cannot be null (Parameter 'transaction')`.

### Root Cause

This was caused due to CreateOrderCommandHandler::Handle() invoking
IntegrationEventLogService::AddAndSaveEventAsync() while passing OrderingContext::CurrentTransaction
with a null value. This was caused due to the fact that OrderingContext's private field
currentTransaction is null until initialized by OrderingContext::BeginTransaction(),
which was not invoked by before accessing OrderingContext's currentTransaction.

### Setup

Why was this working (allegedly) fine in the original eShopOnContainers repo and not in
mine? That repo also was fetching OrderingContext's current transaction w/o previously
having invoked OrderingContext::BeginTransaction().

### Solution

The reason is that the original repo implemented an implicit behavour called TransactingBehaviour
which initialized the OrderingContext's currentContext private field by invoking its
BeginTransaction() method. I missed that aspect of the app so in my version there was no
TransactingBehaviour implemented. I was able to get to the root cause by asking Cursor, so
the LLM pointed out the problem for me. Now, this isn't ideal. I should have been more
tenacious in the quest on solving this problem. The path that I could have taken to solve
this problem on my own is the following:

- Notice an exception was thrown due to the parameter `transaction` being null on
  invoking `IntegrationEventService::SaveEventAsync()`
- This leads to the conclusion in the the invocation of that method, the second arguement
  being used (`orderingContext.CurrentTransaction`) was null.
- This begs the question: what initializes OrderingContext's private field? The answer
  is `OrderingContext::BeginTransaction()` which initialized it to
  `Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);`
- This begs the question: Why that method isn't invoked before access the current transaction?
  The answer is revealed after doing a search on the places, in the original repo, where
  that method was invoked. The result reveals that it's being called inside a MediatR's
  pipeline behaviour called `TransactionBehaviour`. \*
- Therefore, the reason why our replica of the original app isn't working is because it
  didn't implemented MediatR's pipeline behaviour `TransactionBehaviour`.

* THIS IS THE POINT AT WHICH WE ASKED THE LLM FOR SOLUTIONS, INSTEAD OF PUSHING FORWARD OURSELVES.

## Problem #2: EF Core errored on returning ReadOnlyCollection instead of IEnumerable on a field-backed property

### Symptoms

System.InvalidOperationException: No coercion operator is defined between types 'System.
Collections.Generic.List`1[eShop.Services.Ordering.Domain.Model.BuyerAggregate.PaymentMethod]' and
'System.Collections.ObjectModel.ReadOnlyCollection`1[eShop.Services.Ordering.Domain.Model.BuyerAggregate.PaymentMethod]'

### Root Cause

I changed the return type of Buyer::PaymentMethods() from IEnumerable<PaymentMethod> to
ReadOnlyCollection<PaymentMethod> because I thought that this implementation would increase
encapsulation and prevent external code from mutating the return collection. This would lead
to subtle but nasty bugs in which the state of the returned collection would be out-of-sync
with the actual internal state of the object that returned that snapshot of its collection.

### Solution

The solution was simply to go back to returning IEnumerable<PaymentMethod>.

### Problem #3

### Symptoms

NullReferenceException at line 121 of existingOrderItem.CurrentDiscount in Order::AddOrderItem()

### Root Cause

When copying the conditionals from the reference eShopOnContainers repo I tried to avoid
conditionals that evaluated negative assertions (like something being not equal to something
else) because asserting positive conditions is easier to understand. Therefore, I first wrote
the code that should only execute when a variable is null but forgot to add the code that
should only execute when that variable is not null inside an else statement. As a consequence,
that piece of code threw a NullReferenceException when dereferencing that null variable in order
to access one of that object's properties.

### Solution

Put the code that should only execute when that variable is null inside a conditional that
evaluates that condition.
