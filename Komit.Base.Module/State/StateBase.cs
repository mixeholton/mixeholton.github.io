using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Komit.Base.Module.State;

[Index(nameof(TenantId))]
public abstract class StateBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    private int _tenantId;
    [Range(1, int.MaxValue)]
    public int TenantId 
    { 
        get 
        {
            if (_tenantId < 1)
                throw new InvalidOperationException($"TenantId cannot be unknown, was for Type: { GetType().Name }: Id: {Id}");
            return _tenantId; 
        } 
        set
        {
            if (_tenantId != default && _tenantId != value)
                throw new InvalidOperationException($"TenantId cannot be changed, was attempted for Type: { GetType().Name }: Id: {Id}");
            _tenantId = value; 
        }
    }
    [Timestamp]
    public byte[] Timestamp { get; set; }
}