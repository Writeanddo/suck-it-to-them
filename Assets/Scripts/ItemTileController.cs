using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTileController : MonoBehaviour
{

    public GameObject child_tile;
    public GameObject child_image;
    public GameObject child_text;
    public Sprite[] sprites;

    public abstract class State
    {
        public abstract State step(float floor_height, ItemTileController tile_controller);
        public abstract float Width();
        public abstract float Height();

        public float y_pos = 0.0f;
    }

    class Squishing : State
    {
        public float max_squish = 0.2f;
        public float squishedness = 0.0f;

        public override State step(float floor_height, ItemTileController tile_controller)
        {
            if (y_pos > floor_height)
            {
                SquishingFalling new_state = new SquishingFalling();

                return new_state.step(floor_height, tile_controller);
            }
            else if (y_pos < floor_height)
            {
                y_pos = floor_height;
            }

            squishedness += 5.0f * Time.deltaTime;
            if (squishedness >= 1.0f)
            {
                State new_state = new Settled();
                new_state.y_pos = y_pos;

                return new_state;
            }

            return this;
        }

        public override float Width()
        {
            float to_be_squared = (squishedness - 1);
            float sin = Mathf.Sin(to_be_squared * to_be_squared * Mathf.PI);
            return 1.0f + 0.1f * sin * sin;
        }

        public override float Height()
        {
            float to_be_squared = (squishedness - 1);
            float cos = Mathf.Cos(to_be_squared * to_be_squared * Mathf.PI);
            return 0.8f + 0.2f * cos * cos;
        }
    }

    class SquishingFalling : State
    {
        public float max_squish = 0.2f;
        public float squishedness = 0.0f;
        public float y_vel = 0.0f;

        public override State step(float floor_height, ItemTileController tile_controller)
        {
            float to_be_squared;
            float cos;
            float squish_factor;
            float squish_unsquish;

            if (y_pos <= floor_height)
            {
                Squishing new_state = new Squishing();
                // Do the opposite math here to make sure it resquishes as it lands
                to_be_squared = (squishedness - 1);
                cos = Mathf.Cos(to_be_squared * to_be_squared * Mathf.PI);

                squish_factor = cos * cos;

                squish_unsquish = -Mathf.Sqrt((Mathf.Acos(-Mathf.Abs(cos))) / Mathf.PI) + 1.0f;

                new_state.squishedness = squish_unsquish;
                new_state.max_squish = max_squish;

                return new_state.step(floor_height, tile_controller);
            }
            // else if (y_pos < floor_height)
            // {
            //     y_pos = floor_height;
            // }

            // Do some weird math here to make sure it is unsquishing as it falls
            to_be_squared = (squishedness - 1);
            cos = Mathf.Cos(to_be_squared * to_be_squared * Mathf.PI);

            squish_factor = cos * cos;

            squish_unsquish = -Mathf.Sqrt((Mathf.Acos(Mathf.Abs(cos))) / Mathf.PI) + 1.0f;

            squishedness = squish_unsquish;
            squishedness += 5.0f * Time.deltaTime;

            y_vel -= 100.0f * Time.deltaTime;
            y_vel = Mathf.Max(y_vel, -10000.0f);

            y_pos += y_vel * Time.deltaTime;

            if (squishedness >= 1.0f)
            {
                Falling new_state = new Falling();

                new_state.y_vel = y_vel;
                new_state.y_pos = y_pos;

                return new_state;
            }

            return this;
        }

        public override float Width()
        {
            float to_be_squared = (squishedness - 1);
            float sin = Mathf.Sin(to_be_squared * to_be_squared * Mathf.PI);
            return 1.0f + 0.1f * sin * sin;
        }

        public override float Height()
        {
            float to_be_squared = (squishedness - 1);
            float cos = Mathf.Cos(to_be_squared * to_be_squared * Mathf.PI);
            return 0.8f + 0.2f * cos * cos;
        }
    }

    class Falling : State
    {
        public float y_vel = 0.0f;

        public override State step(float floor_height, ItemTileController tile_controller)
        {
            if (y_pos <= floor_height)
            {
                Squishing new_state = new Squishing();
                new_state.y_pos = floor_height;
                // TODO see what's going on here
                new_state.max_squish = Mathf.Max(y_vel * 1e3f, 0.2f);
                return new_state;
            }

            y_vel -= 100.0f * Time.deltaTime;
            y_vel = Mathf.Max(y_vel, -10000.0f);

            y_pos += y_vel * Time.deltaTime;

            return this;
        }

        public override float Width()
        {
            return 1.0f;
        }

        public override float Height()
        {
            return 1.0f;
        }
    }

    class SquishingRising : State
    {
        float squishedness = 0.0f;
        public override State step(float y_height, ItemTileController tile_controller)
        {
            y_pos += 7.5f * Time.deltaTime;
            squishedness = Mathf.Max(squishedness + 5.0f * Time.deltaTime, 1.0f);

            if (y_pos > 1.0f)
            {
                tile_controller.cleanup = true;
                return this;
            }

            return this;
        }

        public override float Width()
        {
            return 0.8f + 0.2f * Mathf.Cos(squishedness * squishedness * Mathf.PI / 2);
        }

        public override float Height()
        {
            return 1.0f + 0.1f * Mathf.Sin(squishedness * squishedness * Mathf.PI / 2);
        }
    }

    class SquishingPooping : State
    {
        float squishedness = 0.0f;
        public override State step(float y_height, ItemTileController tile_controller)
        {
            y_pos -= 2.5f * Time.deltaTime;
            squishedness = Mathf.Max(squishedness + 5.0f * Time.deltaTime, 1.0f);

            if (y_pos < -1.0f)
            {
                tile_controller.cleanup = true;
                return this;
            }

            return this;
        }

        public override float Width()
        {
            return 0.8f + 0.2f * Mathf.Cos(squishedness * squishedness * Mathf.PI / 2);
        }

        public override float Height()
        {
            return 1.0f + 0.1f * Mathf.Sin(squishedness * squishedness * Mathf.PI / 2);
        }
    }

    class Settled : State
    {
        public override State step(float floor_height, ItemTileController tile_controller)
        {
            if (floor_height < y_pos)
            {
                Falling new_state = new Falling();
                new_state.y_pos = y_pos;
                return new_state;
            }
            else if (floor_height > y_pos)
            {
                y_pos = floor_height;
            }
            return this;
        }

        public override float Width()
        {
            return 1.0f;
        }

        public override float Height()
        {
            return 1.0f;
        }
    }

    public void set_y_pos(float y_pos)
    {
        this.state.y_pos = y_pos;
    }

    public bool cleanup = false;

    public State state = new Settled();

    Suckable suckable = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Eject(bool up)
    {
        State new_state;
        if (suckable.isDash)
            new_state = new SquishingPooping();
        else
            new_state = new SquishingRising();

        new_state.y_pos = this.state.y_pos;
        this.state = new_state;
    }

    float get_nominal_y_size(int bag_size)
    {
        return (float)this.suckable.size / (float)bag_size;
    }

    public void Setup(Suckable suckable, float y_pos)
    {
        this.suckable = suckable;
        this.state.y_pos = y_pos;

        child_image.GetComponent<Image>().sprite = suckable.sprite;
    }

    public void Squish()
    {
        this.state = new Squishing();
    }

    public float get_y_factor()
    {
        return this.state.Height();
    }

    public float get_y_size(int bag_size)
    {
        return get_nominal_y_size(bag_size) * this.state.Height();
    }

    public float get_top_pos(int bag_size)
    {
        return this.state.y_pos + get_y_size(bag_size);
    }

    public void SetPosition(float y_min, float nominal_y_size)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        // Change the anchor y min and anchor y max
        // Add a tiny bit of padding
        float y_size = nominal_y_size * get_y_factor();
    }

    public void update_state(float floor_height)
    {
        this.state = this.state.step(floor_height, this);
    }

    public void update_position(int bagSize, float x_center, float nominal_x_width)
    {

        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform child_rect_transform = child_tile.GetComponent<RectTransform>();

        float y_size = this.get_y_size(bagSize);

        float x_width = nominal_x_width * this.state.Width();
        float x_min = x_center - x_width / 2.0f;
        float x_max = x_center + x_width / 2.0f;

        // Update only y pos for me, and only x pos for child
        rectTransform.anchorMin = new Vector2(0, state.y_pos);
        rectTransform.anchorMax = new Vector2(1, state.y_pos + get_nominal_y_size(bagSize));

        float nom_y_size = get_nominal_y_size(bagSize);

        child_rect_transform.anchorMin = new Vector2(x_min, 0.0f + 0.01f / nom_y_size);
        child_rect_transform.anchorMax = new Vector2(x_max, this.state.Height() - 0.01f / nom_y_size);

        // child_rect_transform.offsetMin = new Vector2(0.0f, 10.0f);
        // child_rect_transform.offsetMax = new Vector2(0.0f, -10.0f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetSprite(Suckable.SuckableType suckableType)
    {
        child_tile.GetComponent<Image>().sprite = sprites[(int)suckableType];
    }
    public void SetText(string text)
    {
        child_text.GetComponent<TextMeshProUGUI>().text = text;
    }
}