'use strict';
import express from "express";
import hdbext from "@sap/hdbext"

import * as xsenv from "@sap/xsenv"
import * as routes from "./routes.js"

export default class ExpressServer {
    constructor() {
        xsenv.loadEnv();
        this.port = parseInt(process.env.PORT) || 4_000
        if (!(/^[1-9]\d*$/.test(this.port.toString()) && 1 <= 1 * this.port && 1 * this.port <= 65_535)) {
            throw new Error(`${this.port} is not a valid HTTP port value`)
        }

        
        this.baseDir = process.cwd()
        this.routes = []
        this.app = express()
        
        this.app.express = express
    }

    async start() {
        let app = this.app
        app.baseDir = this.baseDir
        let hanaOptions = xsenv.getServices({
            hana: 'hdi_container'
        })
        
        hanaOptions.hana.pooling = true
        app.use(
            hdbext.middleware(hanaOptions.hana)
        )

        //Load routes
        // TODO: get rid of hello world message
        app.get('/', (req, res) => {
            res.send('Hello world!' + hanaOptions.hana.host + ':' + hanaOptions.hana.port)
        })
        routes.loadRoutes(app)

        this.httpServer = app.listen(this.port)
        let serverAddr = `http://localhost:${this.port}`
        console.log(`Express Server listening on ${serverAddr}`)
        // open(serverAddr)
    }

    stop() {
        this.httpServer.close()
    }
}