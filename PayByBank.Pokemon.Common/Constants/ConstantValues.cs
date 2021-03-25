namespace PayByBank.Pokemon.Common.Constants
{
    public static class ConstantValues
    {
        public const string PokemonApi = "PokemonApi";
        public const string TranslationApi = "TranslationApi";
        public const string Error_NoName = "A name must be specified";
        public const string Error_NotFound = "Unable to find pokemon {0}";
        public const string Error_InternalError_Client = "Sorry there has been an error";
        public const string Error_InternalError_Controller = "Internal Server Error (controller)";
        public const string Error_InternalError_Service = "Internal Server Error (service)";
        public const string Error_InternalError_Repository = "Internal Server Error (repository)";
        public const string Error_InternalError_Adapter = "Internal Server Error (adapter)";
        public const string Error_ExternalApi_Result = "Failed to fetch {0} with result: {1}";
        public const string Error_ExternalApi_Exception = "Failed to fetch {0} with exception: {1}";
        public static readonly double[] smallBucketsSeconds = { 0.0005, 0.001, 0.0025, 0.005, 0.007, 0.009, 0.0125, 0.02, 0.04, 0.08, 0.12, 0.15, 0.200, 0.250, 0.5, 1, 5, 10 };
    }
}
