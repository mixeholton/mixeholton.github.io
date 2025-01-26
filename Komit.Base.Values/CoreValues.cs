namespace Komit.Base.Values;
public record KeyValue(string Key, string Value);
public record IdNamePair(Guid Id, string Name);
public record Message(string Title, string Details);