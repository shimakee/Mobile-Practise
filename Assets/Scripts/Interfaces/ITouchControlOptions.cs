public interface ITouchControlOptions
{
    TouchControlOption TouchOption { get; }
    bool enablePassiveSelection { get; }
    bool enableUnniqueSelection { get; }
    bool enableLastTouchConfirm { get; }
    bool enableDiselectOnlyOnTouchOff { get; }
}