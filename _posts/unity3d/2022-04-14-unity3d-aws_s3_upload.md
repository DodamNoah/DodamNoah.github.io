---
title:  "[Unity3D] Unity3D AWS S3 파일/폴더 업로드"
excerpt: "Unity3D Editor에서 파일 및 폴더를 AWS S3에 업로드하는 법"
date:   2022-04-14 10:30:00 +0530
categories:
  - Unity3D
tags:
  - Unity3D
  - C#
  - AWS
  - CloudFront
toc: true
toc_sticky: true
---
유니티에서 파일/폴더를 아마존 S3에 올린 뒤 무효화(Invalidation)를 진행하여 CloudFront에 바로 반영되는 방법을 알아보도록 하겠습니다.
이번 포스트를 통해서 작성되는 AWSManager는 하기 링크에서 확이 바랍니다.  

**[AWSManager 클래스 코드](/assets/file/AWSManager.cs)**  

해당 내용은 Unity3D 2018.4.10f1에서 진행되었습니다. 

## 1. 선행 작업  
* AWS DLL 추가  
예전(Unity3D 5.x)에는 AWS Unity3D SDK가 제공되었던거 같으나, 더이상 존재하지 않는거 같습니다.  
우선, Unity3D Editor에서만 진행하는 부분이므로 Visual Studio Nuget Packages를 통해서 하기 스샷과 같이 AWSSDK 3개를 추가합니다.  
![Visual Studio Nuget Package](/assets/images/unity/aws/nuget_package.png)  
패키지 설치시에 Assembly Editor에만 추가하면 됩니다.  
![Visual Studio Nuget Package Assembly](/assets/images/unity/aws/nuget_package_add.png)  
마지막으로 설치가 완료되면 설치된 폴더 net45내의 dll를 유니티 프로젝트 Assets/Plugins/x86_64에 추가합니다.  
*MAC 패키지 설치 위치 : ~/.nuget/packages*  
*Windows 패키지 설치 위치 : %userprofile%\.nuget\packages*  

* AWS IAM Action 추가  
업로드 후 무효화(Invalidation)를 진행할 경우 사용하는 Amazon API(CreateInvalidation)에 대해서 Amazon 로그인 후 IAM의 사용할 S3에 Action(cloudFront:CreateInvalidation)을 추가합니다.  
추가하지 않을 경우 **"Amazon.CloudFront.Model.AccessDeniedException: User: arn:aws:iam::xxx:user/xxx is not authorized to perform:"**와 같은 오류가 발생합니다.  
![Amazon S3 IAM](/assets/images/unity/aws/amazon_s3_info.png)
* Unity3D Project 속성 확인  
제가 작성한 코드는 C#의 async, await를 통해서 비동기로 처리하므로 유니티 프로젝트 속성해서 Scripting Runtime Version을 .Net 4.x Equivalent로 처리하여 async, await가 사용 가능하도록 처리합니다.  
![Visual Studio Nuget Package Assembly](/assets/images/unity/aws/unity_project_net.png)
## 2. 파일/폴더 업로드
* 파일 업로드  
TransferUtilityUploadRequest에 넘겨는 버킷명 인자에 루트가 아닌 서브 경로를 추가로 할 경우 처리하는 방식에 주의하셔야 합니다.  
```cs
private void UploadFile(UploadInfo uploadInfo)
{
    _currentUploadInfo = uploadInfo;

    try
    {
        string serviceUrl = _awsInfo.ServiceUrl;
        string subDestPath = _awsInfo._bucket;
        // 서브 경로 얻어오기
        System.Uri uri = new System.Uri(serviceUrl);
        string pathAndQuery = uri.PathAndQuery;

        if (!string.IsNullOrEmpty(pathAndQuery))
        {
            // 버킷의 루트가 아닌 서브 경로에 올릴 경우 처리 
            subDestPath = System.IO.Path.Combine(subDestPath, pathAndQuery);
        }
        if (!string.IsNullOrEmpty(_currentUploadInfo.DestDir))
        {
            // 업로드하는 파일에 추가 서브 경로가 있을 경우 처리 
            subDestPath = System.IO.Path.Combine(subDestPath, _currentUploadInfo.DestDir);
        }        
        // 업로드 객체 생성
        TransferUtilityUploadRequest request = new TransferUtilityUploadRequest()
        {
            BucketName = subDestPath,
            FilePath = _currentUploadInfo._path
        };
        // 업로드 진행 콜백 등록
        request.UploadProgressEvent += UploadProgressCallback;
        // 업로드 결과 콜백
        if (null != _uploadCallback)
        {
            _uploadCallback(AmazonState.UploadStart, AmazonUploadType.File, null, _currentUploadInfo);
        }
        // 업로드
        _transferUtility.Upload(request);
    }
    catch (System.Exception exception)
    {
        // 예외 처리
        UploadException(exception, _currentUploadInfo);
    }
    finally
    {
        // 무효화 요청
        Invalidate(_currentUploadInfo.InvalidPath(_awsInfo.SubPath), _currentUploadInfo);
    }
}
```
* 폴더 업로드  
파일 업로드와 달리 TransferUtilityUploadDirectoryRequest는 루트가 아닐 경우 KeyPrefix에 서브 경로를 지정해야 합니다.   
```cs
private void UploadDirectory(UploadInfo uploadInfo)
{
    _currentUploadInfo = uploadInfo;

    try
    {
        string serviceUrl = _awsInfo.ServiceUrl;
        string subDestPath = string.Empty;
        System.Uri uri = new System.Uri(serviceUrl);
        string pathAndQuery = uri.PathAndQuery;

        if (!string.IsNullOrEmpty(pathAndQuery))
        {
            subDestPath = pathAndQuery;
        }
        if (!string.IsNullOrEmpty(_currentUploadInfo.DestDir))
        {
            subDestPath = System.IO.Path.Combine(subDestPath, _currentUploadInfo.DestDir);
        }

        TransferUtilityUploadDirectoryRequest request = new TransferUtilityUploadDirectoryRequest()
        {
            BucketName = _awsInfo._bucket,
            Directory = _currentUploadInfo._path,
            KeyPrefix = subDestPath,
            SearchOption = System.IO.SearchOption.AllDirectories
        };

        request.UploadDirectoryProgressEvent += UploadProgressCallback;

        if (null != _uploadCallback)
        {
            _uploadCallback(AmazonState.UploadStart, AmazonUploadType.Directory, null, _currentUploadInfo);
        }

        _transferUtility.UploadDirectory(request);
    }
    catch (System.Exception exception)
    {
        UploadException(exception, _currentUploadInfo);
    }
    finally
    {
        Invalidate(_currentUploadInfo.InvalidPath(_awsInfo.SubPath), _currentUploadInfo);
    }
}
```
## 3. 업로드 무효화(Invalidation)  
AWS S3을 통한 CloudFront를 사용할 경우 동일한 경로/이름의 파일 및 폴더를 업데이트 한 경우 기존 항목에 대한 파일 무효화를 진행하여야 바로 반영이 됩니다. 무효화는 파일 및 폴더를 업로드 성공시에 바로 처리하면 됩니다. 그렇지 않을 경우 특정 시간뒤의 갱신이 되며, 자세한 내용은 하기 문서를 참고 바랍니다.   
* [AWS CloudFront Invalidation](https://docs.aws.amazon.com/ko_kr/AmazonCloudFront/latest/DeveloperGuide/Invalidation.html)  
```cs
// invalidPath : bucket 저장소의 루트부터의 폴더명 또는 파일명, subPath가 있을 경우 subPath가 포함된 경우
// 예) subPath : /test/aaa일 경우, /test/aaa/폴더, test/aaa/bbb.txt
private void Invalidate(string invalidPath, UploadInfo uploadInfo)
{
    try
    {
        // AWS accessKey, secretKey, serviceUrl를 통해서 Config를 설정합니다.
        AmazonCloudFrontClient amazonCloudFrontClient = new AmazonCloudFrontClient(_awsInfo._accessKey, _awsInfo._secretKey, new AmazonCloudFrontConfig
        {
            ServiceURL = _awsInfo.ServiceUrl, // "https://" + _bucket + ".s3.amazonaws.com" + SubPath
            RegionEndpoint = RegionEndpoint.GetBySystemName(_awsInfo._region),
            BufferSize = 65536
        });
        // 하기 API처리를 위해서 AWS IAM 정책 Action > cloudfront:CreateInvalidation 추가 
        var result = amazonCloudFrontClient.CreateInvalidation(new CreateInvalidationRequest()
        {
            DistributionId = _awsInfo._distributeId, // CloudFront Id
            InvalidationBatch = new InvalidationBatch
            {
                Paths = new Paths
                {
                    Quantity = 1,
                    Items = new System.Collections.Generic.List<string> { invalidPath }
                },
                CallerReference = System.DateTime.Now.Ticks.ToString(),
            }
        });

        amazonCloudFrontClient.Dispose();
    }
    catch (System.Exception e)
    {
        Debug.LogError(string.Format("Invalidation Exception:::: {0}", e.Message));
    }
}
```
## 4. 업로드 및 무효화 호출  
업로드 할 폴더 및 파일의 정보를 Queue로 작성한 뒤 위에서 작성한 AWSManager 객체에 넘겨서 업로드합니다.  
```cs
Queue<AWSManager.UploadInfo> uploadInfoQueue = new Queue<AWSManager.UploadInfo>();
//폴더일 경우 destDir를 설정해야 해당 정보를 가지고 Bucket에 하위 폴더에 올라갑니다.   
//destDir을 지정하지 않을 경우 원본 폴더내의 내용이 루트에 업로드 됩니다.  
uploadInfoQueue.Enqueue(new AWSManager.UploadInfo(string.Format("{0}{1}", Application.dataPath.Replace("Assets", ""), "compress/test"), "test"));
//파일일 경우  
uploadInfoQueue.Enqueue(new AWSManager.UploadInfo(string.Format("{0}{1}", Application.dataPath.Replace("Assets", ""), "compress/test.zip")));
// AWSManager 생성  
AWSManager awsManager = new AWSManager();
awsManager.Upload(uploadInfoQueue);
```
## 5. 기타
* 비동기 처리  
작성된 코드와 같이 파일 업로드에 대한 프로그래스 및 결과창은 EditorUtility.DisplayDialog와 EditorUtility.DisplayProgressBar를 사용하였으나, 업로드 진행을 async, await를 사용한 비동기 방식으로 처리를 하였으므로 바로 처리시 메인 UI 쓰레드를 사용하라는 오류가 발생합니다.  
이를 위해서 하기와 같이 메인 UI 쓰레드에서 동작하도록 처리를 하도록 합니다.  
```cs
var syncContext = System.Threading.SynchronizationContext.Current;
syncContext.Post((_) =>
{
    EditorUtility.DisplayDialog("Amazon Upload", string.Format("{0}\n\n{1}", uploadInfo._path, message), "OK");
}, "success");
```  
물론 MonoBehaviour를 통한 이벤트를 등록 후 Update에서 처리를 하여도 상관은 없지만, 이번 내용은 유니티 에디터에서 실행이 없이 Inspector에 메뉴를 생성하여 처리를 하므로 MonoBehaviour방식은 불가합니다.  

***참고로 위의 내용은 C#으로 진행되었으므로 C# 어플리케이션 개발에도 활용할 수 있습니다.***🧐 