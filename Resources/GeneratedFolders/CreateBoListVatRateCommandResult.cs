using System;

namespace AccountStructureManagementManagement.Application.Commands.BoListVatRateCommands.CreateBoListVatRate;
public class CreateBoListVatRateCommandResult
{
    public decimal TrSeqId { get; set; }
    public int VatRateId { get; set; }
    public int Vat { get; set; }
    public string Definition { get; set; }
    public bool Active { get; set; }
    public System.DateTime? NewVatDate { get; set; }
    public int? VatNo { get; set; }
    public short OpType { get; set; }
    public System.DateTime OpDate { get; set; }
    public string OpuCode { get; set; }
    public short TrOpType { get; set; }
    public System.DateTime? TrDate { get; set; }
    public bool? TrStatus { get; set; }
    public string TrFileName { get; set; }
    public string SourceType { get; set; }
}
