using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDCBot
{
    class Player
    {
        public int user_id;
        public string username;
        public long count300;
        public int count100;
        public int count50;
        public int playercount;
        public long ranked_score;
        public long total_score;
        public int pp_rank;
        public float level;
        public float pp_raw;
        public float accuracy;
        public int count_rank_ss;
        public int count_rank_s;
        public int count_rank_a;
        public string country;
        public int pp_country_rank;
        public string[] events;
    }
}
/*
[{
	"user_id": "9401092",
	"username": "nitprudo",
	"count300": "186490",
	"count100": "36607",
	"count50": "6095",
	"playcount": "2040",
	"ranked_score": "111777284",
	"total_score": "394682900",
	"pp_rank": "469169",
	"level": "39.2237",
	"pp_raw": "245.137",
	"accuracy": "92.3792495727539",
	"count_rank_ss": "1",
	"count_rank_s": "30",
	"count_rank_a": "40",
	"country": "US",
	"pp_country_rank": "68130",
	"events": []
}]
*/