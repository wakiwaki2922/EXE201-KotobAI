import { XIcon } from "lucide-react";
import Image from "next/image";
import { useCallback, useState } from "react";
import { useDropzone } from "react-dropzone";
import { Button } from "./ui/button";

interface UploadFileProps {
  className: string;
  id: string;
  onUpload: (id: string, file: File) => void;
}

const UploadFile = ({ className, id, onUpload }: UploadFileProps) => {
  const [files, setFiles] = useState<File[]>([]);

  const onDrop = useCallback((acceptedFiles: File[]) => {
    if (acceptedFiles?.length) {
      setFiles((previousFiles) =>
        [...previousFiles, ...acceptedFiles].map((file) =>
          Object.assign(file, {
            preview: URL.createObjectURL(file),
            isVideo: file.type.startsWith('video/')
          })
        )
      );
    }
  }, []);

  const handleUpload = () => {
    files.forEach((file) => {
      onUpload(id, file);
    });
    setFiles([]);
  };

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    multiple: false,
  });

  const removeFile = (fileName: string) => {
    setFiles((previousFiles) =>
      previousFiles.filter((file) => file.name !== fileName)
    );
  };

  return (
    <>
      <div {...getRootProps()} className={className}>
        <input {...getInputProps()} />
        {isDragActive ? (
          <p>Drop the files here ...</p>
        ) : (
          <p className="text-sm text-muted-foreground">
            Drag and drop some files here, or click to select files
          </p>
        )}
      </div>

      {/* Preview files */}
      <ul>
        {files.map((file) => (
          <li key={file.name}>
            <div className="flex items-center justify-between mb-2">
              <Button
                type="button"
                title="Remove file"
                onClick={() => removeFile(file.name)}
                variant="ghost"
                className="top-0 right-0 flex"
              >
                <XIcon className="h-4 w-4" />
                <p>Remove</p>
              </Button>
              <Button onClick={handleUpload} disabled={!files.length}>
                Upload
              </Button>
            </div>
            {file.type.startsWith('video/') ? (
              <video
                src={(file as any).preview}
                width={file.size}
                height={file.size}
                className="video-preview"
                controls
                onLoadedData={() => URL.revokeObjectURL((file as any).preview)}
              />
            ) : (file.type.startsWith('image/') && file.size < 5000000) ? (
              <Image
                src={(file as any).preview}
                alt={file.name}
                width={file.size}
                height={file.size}
                style={{ objectFit: "cover" }}
                onLoad={() => URL.revokeObjectURL((file as any).preview)}
              />
            ) : (
              <div className="text-sm text-muted-foreground">
                {file.name} - File type not supported or file size is too large
              </div>
            )}
            <div className="text-sm text-muted-foreground">{file.name}</div>
          </li>
        ))}
      </ul>
    </>
  );
};

export default UploadFile;
