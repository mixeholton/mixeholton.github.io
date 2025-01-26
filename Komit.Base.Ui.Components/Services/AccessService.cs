using Komit.Base.Ui.Components.Components.PageLayout;
using Komit.Base.Values;
using Komit.Base.Values.Cqrs;
using Komit.Infrastructure.CqrsClient;

namespace Komit.Base.Ui.Components.Services;

public class AccessService
{
    protected CqrsClient Client { get; private set; }
    public IKomitPage? Page { get; private set; }
    public IKomitLayout? Layout { get; private set; }
    protected PortalAccessDto Access { get; private set; }
    public IEnumerable<IdNamePair> OrganizationAffiliations { get; private set; } = Enumerable.Empty<IdNamePair>();
    public IEnumerable<IdNamePair> AffiliatedOrganizations { get; private set; } = Enumerable.Empty<IdNamePair>();
    public static readonly IdNamePair Unknown = new(Guid.Empty, "Ukendt");
    public IdNamePair Organization { get; private set; } = Unknown;
    public IdNamePair Affiliation { get; private set; } = Unknown;
    public AccessService(CqrsClient client)
    {
        Client = client.ForModule("system");
    }
    internal async Task GetAccess()
    {
        Access = await Client.Request(new GetPortalAccessRequest()).Result();
        AffiliatedOrganizations = Access.AffiliatedOrganizations;
        OrganizationAffiliations = Access.OrganizationAffiliations;
        Organization = AffiliatedOrganizations.FirstOrDefault(x => x.Id == Access.OrganizationId) ?? Unknown;
        Affiliation = Access.GetAffiliation() ?? Unknown;
    }
    internal async Task ChangeAccess(Guid organizationId, Guid affiliationId)
    {
        await Client.Command(new SetPortalAccessCommand(organizationId, affiliationId)).Success();
        await GetAccess();
        if (Page != null)
            await Page.Refresh();
    }
    internal AccessService OpenPage(IKomitPage page)
    {
        Page = page;
        return this;
    }
    internal AccessService ClosePage(IKomitPage page)
    {
        if (Page == page)
            Page = null;
        return this;
    }
    internal AccessService OpenLayout(IKomitLayout layout)
    {
        Layout = layout;
        return this;
    }
    internal AccessService CloseLayout(IKomitLayout layout)
    {
        if (Layout == layout)
            Layout = null;
        return this;
    }
}
