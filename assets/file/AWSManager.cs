#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;

public enum AmazonState
{
    UploadStart,
    Uploading,
    UploadSuccess,
    UploadFail,
}

public enum AmazonUploadType
{
    File,
    Directory,
}

public class AWSManager
{
    // AWS의 정보 클래스
    class AwsInfo
    {
        public string _bucket { get; set; }
        public string _region { get; set; }
        public string _accessKey { get; set; }
        public string _secretKey { get; set; }
        public string _subPath { get; set; }
        public string _distributeId { get; set; }

        public string SubPath
        {
            get
            {
                return _subPath ?? "";
            }
        }

        public string ServiceUrl
        {
            get
            {
                return "https://" + _bucket + ".s3.amazonaws.com" + SubPath;
            }
        }
    }

    // 업로드하는 폴더/파일의 정보
    public class UploadInfo
    {
        // 업로드할 파일 또는 디렉토리 풀 경로
        public string _path { get; set; }
        // 업로드 후 Invalidation(무효화)를 진행시 업로드 폴더/파일에 추가 경로가 있을 경우 사용
        public string _destDir { get; set; }
        public bool _fileOrDirectory { get; set; }

        public string DestDir
        {
            get
            {
                return _destDir ?? "";
            }
        }

        public string InvalidPath(string awsSubPath)
        {
            if (_fileOrDirectory)
                return System.IO.Path.Combine(awsSubPath, DestDir, System.IO.Path.GetFileName(_path));
            else
                return System.IO.Path.Combine(awsSubPath, DestDir);
        }

        public UploadInfo(string path, string destDir = "")
        {
            _path = path;
            _destDir = destDir;

            System.IO.FileAttributes fileAttributes = System.IO.File.GetAttributes(path);
            _fileOrDirectory = (fileAttributes & System.IO.FileAttributes.Directory) != System.IO.FileAttributes.Directory;
        }
    }

    // 하기 정보는 보안이 필요할 경우 파일 또는 암호화를 통해서 처리한다.
    private AwsInfo _awsInfo = new AwsInfo()
    {
        _bucket = "버킷 이름 입력",
        _region = "ap-northeast-2", // 서비스 이름 입력
        _accessKey = "Access Key 입력",
        _secretKey = "Secret Key 입력",
        _subPath = "하위 디렉토리 입력", // Optional로 미 입력시 해당 버킷의 루트에 업로드 됩니다.
        _distributeId = "CloudFront ID 입력", // Optional로 Invalidation 진행시에 사용합니다.
    };

    private TransferUtility _transferUtility;
    Queue<UploadInfo> _uploadInfoQueue = new Queue<UploadInfo>();
    UploadInfo _currentUploadInfo;
    System.Action<AmazonState, AmazonUploadType, System.EventArgs, UploadInfo> _uploadCallback = null;

    public AWSManager() 
    {
        SetTransferUtility();
    }

    // Amazon S3 설정 및 업로드 객체 생성
    private void SetTransferUtility()
    {
        if (null != _transferUtility)
            _transferUtility = null;

        string serviceUrl = _awsInfo.ServiceUrl;

        AmazonS3Config s3Config = new AmazonS3Config()
        {
            ServiceURL = serviceUrl,
            RegionEndpoint = RegionEndpoint.GetBySystemName(_awsInfo._region),
            BufferSize = 65536,
            UseAccelerateEndpoint = false
        };

        AmazonS3Client s3Client = new AmazonS3Client(_awsInfo._accessKey, _awsInfo._secretKey, s3Config);
        _transferUtility = new TransferUtility(s3Client);
    }

    public void Upload(Queue<UploadInfo> uploadInfoQueue)
    {
        UnityEditor.EditorUtility.ClearProgressBar();

        //AWS에 올리는 방식은 async, await 비동기로 진행하므로 콜백을 넘겨 받아 EditorUtility를 사용시 메인 쓰레드에서 동작할 수 있도록 처리해야 한다.
        //메인 쓰레드에서 얻어와야 하므로 위치 중요!!!!
        var syncContext = System.Threading.SynchronizationContext.Current;

        Upload(uploadInfoQueue, (state, type, eventArgs, uploadInfo) =>
        {
            switch (state)
            {
                case AmazonState.UploadStart:
                    {
                        syncContext.Post((_) =>
                        {
                            UnityEditor.EditorUtility.ClearProgressBar();
                        }, "start");
                    }
                    break;
                case AmazonState.UploadFail:
                    {
                        Debug.Log(string.Format("Upload Amazon Failed!!::: {0}", uploadInfo._path));
                        syncContext.Post((_) =>
                        {
                            string message = string.Format("Upload {0} Failed!!", uploadInfo._fileOrDirectory ? "File" : "Directory");
                            UnityEditor.EditorUtility.DisplayDialog("Amazon Upload", string.Format("{0}\n\n{1}", uploadInfo._path, message), "OK");
                        }, "fail");
                    }
                    break;
                case AmazonState.UploadSuccess:
                    {
                        Debug.Log(string.Format("Upload Amazon Succeed!!::: {0}", uploadInfo._path));
                        syncContext.Post((_) =>
                        {
                            string message = string.Format("Upload {0} Success!!", uploadInfo._fileOrDirectory ? "File" : "Directory");
                            UnityEditor.EditorUtility.DisplayDialog("Amazon Upload", string.Format("{0}\n\n{1}", uploadInfo._path, message), "OK");
                        }, "fail");
                    }
                    break;
                case AmazonState.Uploading:
                    {
                        Amazon.S3.Transfer.UploadProgressArgs args = (Amazon.S3.Transfer.UploadProgressArgs)eventArgs;
                        float progress = (float)args.TransferredBytes / (float)args.TotalBytes;
                        //Debug.Log(string.Format("Upload Amazon Progress:::: {0}, {1}", progress, fileOrDirectoryPath));
                        syncContext.Post((_) =>
                        {
                            UnityEditor.EditorUtility.DisplayProgressBar("Amazon Upload", string.Format("AWS S3 업로드 중..."), progress);
                        }, "progress");

                    }
                    break;
                default:
                    break;
            }
        });
    }

    public async void Upload(Queue<UploadInfo> uploadInfoQueue, System.Action<AmazonState, AmazonUploadType, System.EventArgs, UploadInfo> uploadedCallback)
    {
        _uploadCallback = uploadedCallback;
        _uploadInfoQueue = uploadInfoQueue;

        if (_uploadInfoQueue.Count == 0)
        {
            return;
        }

        do
        {
            var current = _uploadInfoQueue.Dequeue();

            if (current._fileOrDirectory)
            {
                if (!System.IO.File.Exists(current._path))
                {
                    Debug.LogError(string.Format("UploadFile file does not exist::: {0}", current._path));

                    _uploadCallback(AmazonState.UploadFail, AmazonUploadType.File, null, current);
                    continue;
                }

                await Task.Run(() => UploadFile(current));
            }
            else
            {
                if (!System.IO.Directory.Exists(current._path))
                {
                    Debug.LogError(string.Format("UploadDirectory directory does not exist::: {0}", current._path));

                    _uploadCallback(AmazonState.UploadFail, AmazonUploadType.Directory, null, current);
                    continue;
                }

                await Task.Run(() => UploadDirectory(current));
            }

        } while (_uploadInfoQueue.Count > 0);
    }

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

    private void UploadFile(UploadInfo uploadInfo)
    {
        _currentUploadInfo = uploadInfo;

        try
        {
            string serviceUrl = _awsInfo.ServiceUrl;

            string subDestPath = _awsInfo._bucket;

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

            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest()
            {
                BucketName = subDestPath,
                FilePath = _currentUploadInfo._path
            };

            request.UploadProgressEvent += UploadProgressCallback;

            if (null != _uploadCallback)
            {
                _uploadCallback(AmazonState.UploadStart, AmazonUploadType.File, null, _currentUploadInfo);
            }

            _transferUtility.Upload(request);
        }
        catch (System.Exception exception)
        {
            UploadException(exception, _currentUploadInfo);
        }
        finally
        {
            // 무효화 요청
            Invalidate(_currentUploadInfo.InvalidPath(_awsInfo.SubPath), _currentUploadInfo);
        }
    }

    private void UploadException(System.Exception exception, UploadInfo uploadInfo)
    {
        var amazonUploadType = uploadInfo._fileOrDirectory ? AmazonUploadType.File : AmazonUploadType.Directory;
        if (exception is AmazonS3Exception)
        {
            AmazonS3Exception amazonS3Exception = (AmazonS3Exception)exception;
            if (amazonS3Exception.ErrorCode != null)
            {
                if (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId"))
                {
                    Debug.LogError("Amazon upload fail::: InvalidAccessKeyId");
                }
                else if (amazonS3Exception.ErrorCode.Equals("InvalidSecurity"))
                {
                    Debug.LogError("Amazon upload fail::: InvalidSecurity");
                }
                else
                {
                    Debug.LogError(string.Format("Amazon upload fail::: {0}, {1}", amazonS3Exception.ErrorCode, amazonS3Exception.ErrorType));
                }
            }
            else
            {
                Debug.LogException(amazonS3Exception);
            }

            if (null != _uploadCallback)
            {
                _uploadCallback(AmazonState.UploadFail, amazonUploadType, null, uploadInfo);
            }
        }
        else
        {
            Debug.LogError(string.Format("Upload Exception:::: {0}", exception.Message));
            if (null != _uploadCallback)
            {
                _uploadCallback(AmazonState.UploadFail, amazonUploadType, null, uploadInfo);
            }
        }
    }

    private void Invalidate(string invalidPath, UploadInfo uploadInfo)
    {
        var amazonUploadType = uploadInfo._fileOrDirectory ? AmazonUploadType.File : AmazonUploadType.Directory;
        if (string.IsNullOrEmpty(_awsInfo._distributeId))
        {
            _uploadCallback(AmazonState.UploadSuccess, amazonUploadType, null, uploadInfo);
            return;
        }

        try
        {
            AmazonCloudFrontClient amazonCloudFrontClient = new AmazonCloudFrontClient(_awsInfo._accessKey, _awsInfo._secretKey, new AmazonCloudFrontConfig
            {
                ServiceURL = _awsInfo.ServiceUrl,
                RegionEndpoint = RegionEndpoint.GetBySystemName(_awsInfo._region),
                BufferSize = 65536
            });
            // 하기 API처리를 위해서 AWS IAM 정책 Action > cloudfront:CreateInvalidation 추가 
            var result = amazonCloudFrontClient.CreateInvalidation(new CreateInvalidationRequest()
            {
                DistributionId = _awsInfo._distributeId,
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

            Debug.Log(string.Format("invalidate::: {0}, {1}", invalidPath, result.Invalidation.Status));
            _uploadCallback(AmazonState.UploadSuccess, amazonUploadType, null, uploadInfo);

            amazonCloudFrontClient.Dispose();
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("Invalidation Exception:::: {0}", e.Message));
        }
    }

    // 업로드 진행 프로그래스 콜백
    private void UploadProgressCallback(object sender, object args)
    {
        if (args is UploadProgressArgs) // file
        {
            _uploadCallback?.Invoke(AmazonState.Uploading, AmazonUploadType.File, (UploadProgressArgs)args, _currentUploadInfo);
        }
        else if (args is UploadDirectoryProgressArgs) // directory
        {
            _uploadCallback?.Invoke(AmazonState.Uploading, AmazonUploadType.Directory, (UploadDirectoryProgressArgs)args, _currentUploadInfo);
        }
    }
}

#endif