namespace InvertedTomato.LightWeightSerialization {
    public class LightWeightOptions {
        /// <summary>
        /// The buffer size when initially allocated.
        /// </summary>
        public int SerializeBufferInitialSize { get; set; } = 1024; // b

        /// <summary>
        /// Amount to grow the buffer when it's full.
        /// </summary>
        public int SerializeBufferGrowthSize { get; set; } = 256 * 1024; // b

    }
}
