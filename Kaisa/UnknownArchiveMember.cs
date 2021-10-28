namespace Kaisa
{
    /// <summary>Represents an archive member which cannot be parsed by Kaisa.</summary>
    /// <remarks>
    /// The data for this member can be read by seeking the archive's stream's position to <see cref="ArchiveMember.MemberDataStart"/>
    /// and reading up to <see cref="ArchiveMember.Size"/> bytes.
    /// </remarks>
    public sealed class UnknownArchiveMember : ArchiveMember
    {
        internal UnknownArchiveMember(Archive library, ArchiveMemberHeader header)
            : base(library, header)
        { }
    }
}
