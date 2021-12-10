import React, {useState, useEffect} from 'react';
import axios from "axios";
import {Progress, ListGroup, ListGroupItem, Button, ButtonGroup, Input, FormGroup, Label} from "reactstrap";

const Home = () => {
    const [selectedFile, setSelectedFile] = useState(undefined);
    const [files, setFiles] = useState([]);
    const [fileName, setFileName] = useState('');
    const [uploading, setUploading] = useState(false);
    const [uploadProgress, setUploadProgress] = useState(0);

    useEffect(() => {
        axios.get("api/FileLoader").then(r => setFiles(r.data));
    }, [])

    useEffect(() => {
        if (selectedFile !== undefined) setFileName(selectedFile.name);
    }, [selectedFile])

    const fileSelected = (e) => {
        setSelectedFile(e.target.files[0]);
    };

    const uploadFiles = () => {
        const formData = new FormData();
        formData.append("file", selectedFile);
        formData.append("name", fileName);
        setUploadProgress(0);
        setUploading(true);
        console.log(uploading);
        axios.post("api/fileloader", formData, {
            headers: {"Content-Type": "multipart/form-data"}, onUploadProgress: (e) => {
                let pc = Math.round((e.loaded * 100) / e.total);
                setUploadProgress(pc);
            }
        }).then(r => {
            setFiles(s => [...s, r.data.filename]);
            console.log(r);
            setUploading(false);
        }).catch(e => {
            console.error(e);
            setUploading(false);
            setUploadProgress(0);
            setFileName('');
        });
    }
    
    const deleteFile = (filename) => {
        console.log(`${filename} deleting`);
        
        axios.delete(`api/FileLoader/${filename}`).then(r => {
            setFiles(f => f.filter(s => s !== filename));
        });
    }

    const fileList = files.map(f => <ListGroupItem key={f}>
        <div>
            <div className="d-flex w-100 justify-content-between">
                <h5>{f}</h5>
                <ButtonGroup>
                    <Button size="sm" color="danger" onClick={() => deleteFile(f)}>Delete</Button>    
                </ButtonGroup>
                
            </div>
        </div>
    </ListGroupItem>);
    const uploadProgressBar = uploading && <Progress value={uploadProgress}/>;
    return (
        <div>  
            <FormGroup>
                <Label for="fileInput">File</Label>
                <Input type="file" onChange={(e) => fileSelected(e)} id="fileInput"/>
            </FormGroup>
            <FormGroup>
                <Label for="fileName">Filename</Label>
                <Input type="text" value={fileName} onInput={(e) => setFileName(e.target.value)} id="fileName"/>    
            </FormGroup>            
            <Button type="button" onClick={uploadFiles} disabled={!selectedFile}>Upload</Button>
            <hr/>
            <div>
                {uploadProgressBar}
            </div>
            <hr/>
            {files.length > 0 ? <ListGroup>{fileList}</ListGroup> : <h5>Load some files to get started</h5>}
            <hr/>
            {uploading}
        </div>
    )
}

export default Home;
