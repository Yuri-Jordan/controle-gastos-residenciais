import axios, { AxiosInstance, AxiosError } from 'axios';
import {  
    Pessoa, CriarPessoa, AtualizarPessoa,
    Categoria, CriarCategoria,
    Transacao, CriarTransacao,
    RelatorioTotaisPessoa, RelatorioTotaisCategoria
} from '../types';

// Configuração do cliente da API
class ClienteApi {
    private cliente: AxiosInstance;

    constructor() {
        this.cliente = axios.create({
            baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5000/api',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        // Interceptor de resposta para tratamento de erros
        this.cliente.interceptors.response.use(
            (response) => response,
            (error: AxiosError) => {
                console.error('Erro na API:', error.response?.data || error.message);
                return Promise.reject(error);
            }
        );
    }

    // Endpoints de Pessoa
    async obterPessoas(): Promise<Pessoa[]> {
        const response = await this.cliente.get<Pessoa[]>('/pessoas');
        return response.data;
    }

    async obterPessoa(id: string): Promise<Pessoa> {
        const response = await this.cliente.get<Pessoa>(`/pessoas/${id}`);
        return response.data;
    }

    async criarPessoa(pessoa: CriarPessoa): Promise<Pessoa> {
        const response = await this.cliente.post<Pessoa>('/pessoas', pessoa);
        return response.data;
    }

    async atualizarPessoa(id: string, pessoa: AtualizarPessoa): Promise<Pessoa> {
        const response = await this.cliente.put<Pessoa>(`/pessoas/${id}`, pessoa);
        return response.data;
    }

    async excluirPessoa(id: string): Promise<void> {
        await this.cliente.delete(`/pessoas/${id}`);
    }

    // Endpoints de Categoria
    async obterCategorias(): Promise<Categoria[]> {
        const response = await this.cliente.get<Categoria[]>('/categorias');
        return response.data;
    }

    async criarCategoria(categoria: CriarCategoria): Promise<Categoria> {
        const response = await this.cliente.post<Categoria>('/categorias', categoria);
        return response.data;
    }

    // Endpoints de Transação
    async obterTransacoes(): Promise<Transacao[]> {
        const response = await this.cliente.get<Transacao[]>('/transacoes');
        return response.data;
    }

    async obterTransacao(id: string): Promise<Transacao> {
        const response = await this.cliente.get<Transacao>(`/transacoes/${id}`);
        return response.data;
    }

    async criarTransacao(transacao: CriarTransacao): Promise<Transacao> {
        const response = await this.cliente.post<Transacao>('/transacoes', transacao);
        return response.data;
    }

    async excluirTransacao(id: string): Promise<void> {
        await this.cliente.delete(`/transacoes/${id}`);
    }

    async obterTransacoesPorPessoa(pessoaId: string): Promise<Transacao[]> {
        const response = await this.cliente.get<Transacao[]>(`/transacoes/pessoa/${pessoaId}`);
        return response.data;
    }

    // Endpoints de Relatórios
    async obterRelatorioTotaisPorPessoa(): Promise<RelatorioTotaisPessoa> {
        const response = await this.cliente.get<RelatorioTotaisPessoa>('/relatorios/pessoas');
        return response.data;
    }

    async obterRelatorioTotaisPorCategoria(): Promise<RelatorioTotaisCategoria> {
        const response = await this.cliente.get<RelatorioTotaisCategoria>('/relatorios/categorias');
        return response.data;
    }
}

export default new ClienteApi();