using Google.GenAI.Types;
using HappyEat.API.Data;
using HappyEat.API.DTOs;
using HappyEat.API.Exceptions;
using HappyEat.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HappyEat.API.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HappyEatDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        public GeminiService(HappyEatDbContext dbContext, IWebHostEnvironment env, IConfiguration config)
        {
            _dbContext = dbContext;
            _env = env;
            _config = config;
        }
        public async Task<GeminiResponseDto> AnalyzeFoodImageAsync(int imageId, string? userPrompt = null)
        {
            //找實體圖片
            var image = await _dbContext.FoodImages.AsNoTracking().FirstOrDefaultAsync(i=>i.ImageId==imageId);
            if (image == null) throw new NotFoundException("Image Not Found.");
            if (string.IsNullOrWhiteSpace(image.ImagePath)) throw new BusinessException("Food Image Path is empty.");
            var fileName = Path.GetFileName(image.ImagePath);
            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
            if (!System.IO.File.Exists(filePath)) throw new NotFoundException("Image File Not Found.");

            //讀取圖片
            var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = GetMimeType(filePath);

            //取得API key
            var apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey)) throw new BusinessException("Gemini API Key is not configured.");

            //讀取超商食品熱量參考資料
            var storeDataPath = Path.Combine(_env.ContentRootPath, "Data", "StoreProducts.json");
            if (!System.IO.File.Exists(storeDataPath)) throw new NotFoundException("StoreProducts.json Not Found.");
            var storeDataJson = await System.IO.File.ReadAllTextAsync(storeDataPath);

            //建立Gemini Client
            var client = new Google.GenAI.Client(apiKey:apiKey);
            var imagePart = Part.FromBytes(imageBytes, contentType);

            //使用者補充說明
            string promptInstruction = string.IsNullOrWhiteSpace(userPrompt)
                                     ? "請根據照片中的外觀進行辨識與估算"
                                     : $@"【使用者補充說明/修正提示】：""{userPrompt}""請將上述「使用者補充說明」視為最高優先決策依據！
                            （例如：若使用者註明是『燒肉鬆餅』，請勿誤判為甜鬆餅或鮮奶油鬆餅；若註明『無糖綠茶』，請以無糖計算熱量）。";

            string prompt = $@"你是一位專業的營養師與台灣便利商店食品專家。請分析這張食物照片並結合使用者的提示說明：{promptInstruction}

            【辨識與比對邏輯】：
            1. 使用者提示優先

            優先採納「使用者補充說明」提供的食材細節與種類。

            如果使用者明確指出食品名稱、飲料種類、糖度、
            份量等資訊，請以使用者提供的資訊為主要依據。


            2. 超商商品比對

            請比對「超商參考商品庫」。

            如果照片中的食品看起來是 7-ELEVEN、
            全家等便利商店商品，請特別留意：

            - 商品名稱
            - 包裝設計
            - 印刷文字
            - 商品外觀
            - 容器形狀

            如果可以在「超商參考商品庫」中找到對應商品：

            - 優先採用資料庫中的官方商品名稱
            - 優先採用資料庫中的官方 Calories
            - Protein / Carbs / Fat 則根據商品資訊、
              包裝資訊與照片進行合理推估

            超商商品的官方 Calories 是主要熱量依據，
            不要因為三大營養素估算結果略有差異，
            而自行修改官方 Calories。


            3. 一般食品

            如果不是超商商品，例如：

            - 傳統便當
            - 路邊攤小吃
            - 家常菜
            - 自製餐點

            請依據照片外觀與專業營養知識，
            估算品項名稱、份量、熱量與三大營養素。


            4. 食品拆分

            請將照片中的所有獨立食品與飲品
            拆分成個別品項。


            5. 品項名稱與數量

            請精確拆分品項名稱、數量與單位。

            正確範例：

            名稱：""炸雞塊""
            數量：6
            單位：""塊""

            正確範例：

            名稱：""無糖綠茶""
            數量：1
            單位：""杯""

            錯誤範例：

            名稱：""炸雞塊(6塊)""

            請不要將數量寫進名稱中。


            6. 營養數值的計算基準

            calories、protein、carbs、fat
            必須提供「單一單位」的營養數值。

            例如：

            炸雞塊
            quantity = 6
            unit = ""塊""
            calories = 45
            protein = 3
            carbs = 2
            fat = 3

            代表每 1 塊炸雞塊的營養資訊。

            後端會根據 quantity 計算實際攝取量。


            【一般食品熱量計算規則】

            對於沒有官方超商 Calories 的一般食品：

            Calories 應盡量符合：

            Calories =
            (Carbs × 4) +
            (Protein × 4) +
            (Fat × 9)

            請確保數值彼此合理一致。


            【超商食品規則】

            如果商品可以在超商參考商品庫中找到：

            Calories 請優先使用資料庫中的官方數值。

            Protein、Carbs、Fat
            則依商品資訊、照片與專業營養知識
            進行合理估算。


            【超商參考商品庫】

            {storeDataJson}


            請只回傳指定 JSON 格式。

            不要包含 Markdown 語法，例如 ```json。
            不要加入額外的開頭或結尾說明。";

            //JSON Schema
            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseJsonSchema = new Schema
                {
                    Type = Google.GenAI.Types.Type.Object,
                    Properties = new Dictionary<string, Schema>
                    {
                        //{ "totalCalories", new Schema { Type = Google.GenAI.Types.Type.Integer, Description = "整餐總熱量 (kcal)" } },
                        //{ "totalProtein", new Schema { Type = Google.GenAI.Types.Type.Number, Description = "整餐總蛋白質 (g)" } },
                        //{ "totalCarbs", new Schema { Type = Google.GenAI.Types.Type.Number, Description = "整餐總碳水化合物 (g)" } },
                        //{ "totalFat", new Schema { Type = Google.GenAI.Types.Type.Number, Description = "整餐總脂肪 (g)" } },
                        { "description", new Schema { Type = Google.GenAI.Types.Type.String, Description = "營養師的飲食建議與評語" } },
                        { "items", new Schema
                            {
                                Type = Google.GenAI.Types.Type.Array,
                                Description = "這餐包含的所有獨立食物品項清單",
                                Items = new Schema
                                {
                                    Type = Google.GenAI.Types.Type.Object,
                                    Properties = new Dictionary<string, Schema>
                                    {
                                        { "itemName", new Schema { Type = Google.GenAI.Types.Type.String, Description = "品項名稱，例如：炸雞塊、烤雞蛋餅" } },
                                        { "quantity", new Schema { Type = Google.GenAI.Types.Type.Number, Description = "數量，例如：6、1" } },
                                        { "unit", new Schema { Type = Google.GenAI.Types.Type.String, Description = "單位，例如：塊、份、杯、顆、碗" } },
                                        { "calories", new Schema { Type = Google.GenAI.Types.Type.Number, Description = "該品項熱量" } },
                                        { "protein", new Schema { Type = Google.GenAI.Types.Type.Number, Description = "該品項蛋白質" } },
                                        { "carbs", new Schema { Type = Google.GenAI.Types.Type.Number, Description = "該品項碳水化合物" } },
                                        { "fat", new Schema { Type = Google.GenAI.Types.Type.Number, Description = "該品項脂肪" } }
                                    },
                                    Required = new List<string> { "itemName", "quantity", "unit", "calories", "protein", "carbs", "fat" }
                                }
                            }
                        }
                    },
                    Required = new List<string> { "items", "description" }
                }
            };

            //多模型備援+timeout
            var modelsToTry = new[] { "gemini-3.7-flash", "gemini-3.6-flash", "gemini-3.5-flash" };
            GenerateContentResponse? response = null;
            var errorLogs = new List<string>();
            foreach( var model in modelsToTry)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                    response = await client.Models.GenerateContentAsync(
                        model: model,
                        contents: new List<Content>
                            {
                                new Content {Parts=new List<Part>{Part.FromText(prompt),imagePart}}
                            },
                        config: config,
                        cancellationToken: cts.Token
                        );
                    if(response!= null && !string.IsNullOrWhiteSpace(response.Text))
                    {
                        System.Diagnostics.Debug.WriteLine($"[Gemini Success]成功使用模型{model}");
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    var msg = $"[Gemini Timeout] 模型{model}" + $"呼叫超過15秒無回應";
                    System.Diagnostics.Debug.WriteLine(msg);
                    errorLogs.Add(msg);
                }
                catch (Exception ex)
                {
                    var innerMsg = ex.InnerException?.Message?? ex.Message;
                    var msg = $"[Gemini Error] 模型{model} 失敗" + $"{innerMsg}";
                    System.Diagnostics.Debug.WriteLine(msg);
                    errorLogs.Add(msg);
                }
            }
            //所有模型皆失敗
            if(response==null || string.IsNullOrWhiteSpace(response.Text)){
                var combinedErrors = string.Join("|", errorLogs);
                throw new BusinessException($"Gemini is busy...Details:{combinedErrors}");
            }

            //JSON Deserialize
            var deserialzeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };
            var resultDto = JsonSerializer.Deserialize<GeminiResponseDto>(response.Text, deserialzeOptions);
            if (resultDto == null) throw new BusinessException("Gemini returned an incorrect data format.");
            
            //根據quantity計算總營養素
            foreach(var item in resultDto.Items)
            {
                var quantity = item.Quantity;
                item.Calories = Math.Round(item.Calories * quantity, 2);
                item.Carbs = Math.Round(item.Carbs * quantity, 2);
                item.Protein = Math.Round(item.Protein * quantity, 2);
                item.Fat = Math.Round(item.Fat * quantity, 2);
            }
            resultDto.TotalCalories = Math.Round(resultDto.Items.Sum(i => i.Calories),2);
            resultDto.TotalCarbs = Math.Round(resultDto.Items.Sum(i => i.Carbs),2);
            resultDto.TotalProtein = Math.Round(resultDto.Items.Sum(i => i.Protein),2);
            resultDto.TotalFat = Math.Round(resultDto.Items.Sum(i => i.Fat),2);

            //補上圖片ID
            resultDto.ImageId = imageId;

            return resultDto;
        }

        //MIME Type Helper
        private static string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" =>"image/webp",
                ".gif"=> "image/gif",
                _=>"application/octet-stream"
            };
        }
    }
}
