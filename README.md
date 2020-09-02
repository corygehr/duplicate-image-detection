# duplicate-image-detection
Simple, naïve approach to detected duplicated images in a folder.

**This is not optimized.** I created this to solve the problem (semi-) quickly based on a StackOverflow solution (will find and link later). The approach works as follows:

1. Resize each image to consistent, small dimensions.
  * The greater the size, the higher the fidelity **but** at a time cost.
2. Get the brightness value of each pixel in the resized image and create a binary value.
  * Brightness < 50% == 0, >= 50% == 1.
3. With each generated image 'hash', compute the Damerau Levenshtein distance against the other images from the same directory.

Allows for a custom sensitivity ranking if you want to improve detection (at the risk of false positives).
