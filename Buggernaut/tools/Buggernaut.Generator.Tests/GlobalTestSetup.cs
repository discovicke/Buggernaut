// Inaktiverar parallell körning av testklasser så Console.SetOut-omdirigering
// i ExerciseScaffolderTests och CliArgumentParserTests fungerar utan race conditions.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

