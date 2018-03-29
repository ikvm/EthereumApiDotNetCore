pragma solidity ^0.4.21;

import "../erc20Contract.sol";

/* @title InvoiceMaster smartcontract */
contract InvoiceMaster {

    event InvoiceCreated(address indexed caller, uint indexed invoiceId, uint dueDateUnix, uint invoiceAmount);
    event PaymentDetected(address indexed caller, uint indexed invoiceId, uint payedAmount);

    struct Invoice {
        uint128 invoiceId;//Guid in external system
        uint invoiceAmount;
        address tokenAddress;
        address merchantWalletAddress;
        uint dueDateUnixTime;
        uint lastPaymentDateUnixTime;
        uint payedAmount;
        bool isValid;
    }

    //Possible Invoice statuses
    enum InvoiceStatus {UNPAID,PAID,OVERDUE,LATE_PAYMENT,PARTIALLY_PAID}

    address _owner;
    mapping (uint128 => Invoice) public registeredInvoices;

    modifier onlyOwner { 
        if (msg.sender == _owner) _; 
    }

    function InvoiceMaster() public {
        _owner = msg.sender;
    }

    function() public payable {
        revert();
    }

    function createInvoice(uint128 invoiceId, uint invoiceAmount, address tokenAddress, address merchantWalletAddress, uint dueDateUnixTime) onlyOwner public {
        if (registeredInvoices[invoiceId].isValid){
            revert();
        }

        Invoice memory invoice = Invoice(invoiceId, invoiceAmount, tokenAddress, merchantWalletAddress, dueDateUnixTime, 0, 0, true);

        registeredInvoices[invoice.invoiceId] = invoice;
        emit InvoiceCreated(msg.sender, invoiceId, dueDateUnixTime, invoiceAmount);
    }  

    function tokenFallback(address _from, uint _value, bytes _data) public returns(bool ok) {
        address tokenAddress = msg.sender;
        bytes16 data16 = bytesToBytes16(_data);
        uint128 invoiceId = uint128(data16);
        Invoice memory invoice = registeredInvoices[invoiceId];
        
        if (!invoice.isValid){
            revert();
        }

        if (tokenAddress != invoice.tokenAddress || _value == 0){
            revert();
        }

        invoice.lastPaymentDateUnixTime = block.timestamp;
        invoice.payedAmount += _value;

        registeredInvoices[invoiceId] = invoice;
        emit PaymentDetected(tokenAddress, invoiceId, _value);

        return true;
    }

    function isInvoiceCreated(uint128 invoiceId) public view returns (bool created){
        Invoice memory invoice = registeredInvoices[invoiceId];

        return invoice.isValid;
    }

    function getInvoiceInfo(uint128 invoiceId) public view returns (uint lastPaymentDateUnixTime, uint dueDateUnix, uint payedAmount, address merchantWalletAddress){
        Invoice memory invoice = registeredInvoices[invoiceId];

        if (!invoice.isValid){
            revert();
        }

        return (invoice.lastPaymentDateUnixTime, invoice.dueDateUnixTime, invoice.payedAmount, invoice.merchantWalletAddress);
    }

    function getInvoiceStatus(uint128 invoiceId) public view returns (InvoiceStatus status){
        Invoice memory invoice = registeredInvoices[invoiceId];

        if (!invoice.isValid){
            revert();
        }

        uint amount = invoice.invoiceAmount;
        uint balance = invoice.payedAmount;
        uint lastPaymentDate = 0;
        uint dueDate = invoice.dueDateUnixTime;
        
        if (invoice.lastPaymentDateUnixTime == 0){
            lastPaymentDate = now;                
        } else{
            lastPaymentDate = invoice.lastPaymentDateUnixTime;
        }
       
        bool isOverdue = dueDate < lastPaymentDate;

        if (amount <= balance){
            if (!isOverdue){
                return InvoiceStatus.PAID;
            } else{
                return InvoiceStatus.LATE_PAYMENT;
            }
        }

        if (balance == 0){
            if (!isOverdue){
                return InvoiceStatus.UNPAID;
            } else{
                return InvoiceStatus.OVERDUE;
            }
        }

        if (amount > balance){
            if (!isOverdue){
                return InvoiceStatus.PARTIALLY_PAID;
            } else{
                return InvoiceStatus.OVERDUE;
            }
        }  
    }

    function isOverdue(uint128 invoiceId) public view returns (uint dateUnix, bool isOverdueResult, InvoiceStatus statusResult, uint dateUnixDiff){
        Invoice memory invoice = registeredInvoices[invoiceId];

        if (!invoice.isValid){
            revert();
        }
        uint lastPaymentDate = 0;
        uint balance = invoice.payedAmount;
        uint dueDate = invoice.dueDateUnixTime;
        InvoiceStatus status = InvoiceStatus.UNPAID;
        if (invoice.lastPaymentDateUnixTime == 0){
            lastPaymentDate = now;                
        } else{
            lastPaymentDate = invoice.lastPaymentDateUnixTime;
        }
        uint diff = dueDate - lastPaymentDate;
        bool isOverdue = diff < 0;

        if (balance == 0){
            if (!isOverdue){
                status = InvoiceStatus.UNPAID;
            } else{
                status = InvoiceStatus.OVERDUE;
            }
        }

        return (lastPaymentDate, isOverdue, status, diff);
    }

    function getCurrentDate() public view returns (uint dateTimeUnix){
        return now;
    }


    function transferAllTokens(uint128 invoiceId) onlyOwner public returns (bool success) {
        Invoice memory invoice = registeredInvoices[invoiceId];

        if (!invoice.isValid){
            revert();
        }

        ERC20Interface erc20Contract = ERC20Interface(invoice.tokenAddress);
        uint balance = erc20Contract.balanceOf(this); 

        if (balance <= invoice.payedAmount) {
            return false;
        }

        return erc20Contract.transfer(invoice.merchantWalletAddress, invoice.payedAmount);
    }

    function bytesToBytes16(bytes b) public pure returns (bytes16) {
        bytes16 out;

        for (uint i = 0; i < 16; i++) {
            out |= bytes16(b[i] & 0xFF) >> (i * 8);
        }

        return out;
    }

    function bytesToUint(bytes b) public pure returns (uint128) {
        bytes16 data16 = bytesToBytes16(b);
        uint128 invoiceId = uint128(data16);

        return invoiceId;
    }
}   