using UnityEngine;
using System.Collections;

public class NoiseGenerator : MonoBehaviour
{
    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;

    public Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    public bool addRandomPixel = false;
    public Vector2 dimensions;

    public Transform modTransform;

    public Material snowMat;
    public Texture2D testTexture;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;

        CalcNoise();

        dimensions = new Vector2(10 * transform.localScale.x, 10 * transform.localScale.y);
    }
    /*
    private void FlipTexture()
    {
        Texture2D newTex = FlipTextureXY((Texture2D)rend.material.mainTexture);
        rend.material.mainTexture = newTex;
    }

    private Texture2D FlipTextureXY(Texture2D original)
    {
        Color[] colors = original.GetPixels();
        Color[] newColors = original.GetPixels();

        for (int i = colors.Length - 1; i >= 0; i--)
        {
            newColors[newColors.Length - 1 - i] = colors[i];
        }

        Texture2D flippedTexture = new Texture2D(original.width, original.height);
        flippedTexture.SetPixels(newColors);
        flippedTexture.Apply();
        return flippedTexture;
    }
    */

    void CalcNoise()
    {
        // For each pixel in the texture...
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                sample = Mathf.Min(1, sample + .2f);

                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    void AddPixelAt(Vector2Int position)
    {
        int pos = position.y * noiseTex.width + position.x;
        //flip hem
        pos = pix.Length - 1 - pos;

        pix[pos] = Color.red;
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    void FixedUpdate()
    {
        if (addRandomPixel)
        {
            //addRandomPixel = false;
            AddPixelAt(new Vector2Int(pixWidth - 1, pixHeight - 1));

            var targetPos = modTransform.position;
            Vector3 bottomLeft = new Vector3(transform.position.x - 0.5f * dimensions.x, 0, transform.position.z - 0.5f * dimensions.y);

            if (targetPos.x >= bottomLeft.x && targetPos.x < bottomLeft.x + dimensions.x)
            {
                if (targetPos.z >= bottomLeft.z && targetPos.z < bottomLeft.z + dimensions.y)
                {
                    int xPos = Mathf.FloorToInt((targetPos.x - bottomLeft.x) / (dimensions.x / pixWidth));
                    int yPos = Mathf.FloorToInt((targetPos.z - bottomLeft.z) / (dimensions.y / pixHeight));
                    AddPixelAt(new Vector2Int(xPos, yPos));
                    snowMat.SetColor("coll", Color.red);
                }
            }
            /*
            else
            {
                Debug.Log("Niet in x-bereik");
            }*/
        }
    }

    /*
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        var result = new Vector2Int(-1, -1);

        // Zit binnen bereik x
        if (worldPosition.x >= transform.position.x - 0.5f * dimensions.x && worldPosition.x < transform.position.x + 0.5f)
        {
            // Zit binnen bereik z
            if (worldPosition.y >= transform.position.z - 0.5f * dimensions.y && worldPosition.y < transform.position.z + 0.5f)
            {
                float cellWidth = dimensions.x / pixWidth;
                float cellHeight = dimensions.y / pixHeight;

                worldPosition += new Vector3(0.5f * dimensions.x, 0, 0.5f * dimensions.y);

                int xPos = Mathf.FloorToInt(worldPosition.x / cellWidth);
                int yPos = Mathf.FloorToInt(worldPosition.z / cellHeight);
                result = new Vector2Int(xPos, yPos);
            }
        }

        return result;
    }
    */
}