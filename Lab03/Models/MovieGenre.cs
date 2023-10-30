using System.ComponentModel.DataAnnotations;

namespace Lab03.Models
{
    public enum MovieGenre
    {
        None = -1, // 无
        Unknown, // 未知
        Action, // 动作
        Adventure, // 冒险
        Animation, // 动画
        Biography, // 传记
        Comedy, // 喜剧
        Crime, // 犯罪
        Documentary, // 纪录片
        Drama, // 戏剧
        Family, // 家庭
        Fantasy, // 奇幻
        FilmNoir, // 黑色电影
        History, // 历史
        Horror, // 恐怖
        Music, // 音乐
        Musical, // 音乐剧
        Mystery, // 悬疑
        Romance, // 浪漫
        [Display(Name = "Science Fiction")]
        Science, // 科幻
        Sport, // 体育
        Thriller, // 惊悚
        War, // 战争
        Western // 西部
                  
    }


}
